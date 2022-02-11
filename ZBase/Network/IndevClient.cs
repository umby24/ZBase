using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ManagedSockets;
using ManagedSockets.EventArgs;
using ZBase.Common;
using ZBase.World;
using ZBase.Network.Indev;

namespace ZBase.Network
{
    internal class IndevClient : TaskItem, INetworkClient
    {
        public string Ip { get; set; } // -- The IP of the connected client
        public IMinecraftPlayer ClientPlayer { get; set; } // -- The entity tied to this client (If they are verified.)
        public bool Verified { get; set; } // -- True if this is a minecraft client that has been negotiated with us.
        public string Name { get; set; }

        public readonly IndevByteBuffer SendBuffer; // -- The send buffer
        public ConcurrentQueue<IIndevPacket> BlockChanges { get; set; }
        public bool DataAvailable;
        // -- Events
        // -- Connected / Disconnected
        // -- DataRecv, DataSent, Packet Handled.

        // -- Private
        private Dictionary<byte, IIndevPacket> _packets; // -- All recognized packets. 
        private readonly ClientSocket _socket; // -- The raw network socket.
        private readonly IndevByteBuffer _receiveBuffer; // -- The receive buffer
        private DateTime _lastActive; // -- Last time a packet was handled from this client (for timeouts)
        private bool _canReceive; // -- If the client is allowed to send packets to the server or not.
        private bool _disconnectOnSend;
        private readonly object _fk = new object(); // -- Lock to ensure two packets are not being handled at once
        private readonly string _taskId; // -- The task ID for the timeout for this client.
        

        public IndevClient(TcpClient sock)
        {
            DataAvailable = false;
            // -- Instantiate our needed items
            _receiveBuffer = new IndevByteBuffer();
            SendBuffer = new IndevByteBuffer();
            _socket = new ClientSocket();
            BlockChanges = new ConcurrentQueue<IIndevPacket>();
            PopulatePackets();

            // -- Register events
            _socket.DataReceived += SocketOnDataReceived;
            _socket.Disconnected += SocketOnDisconnected;
            SendBuffer.DataAdded += SendBufferOnDataAdded;
            _canReceive = true;
            _disconnectOnSend = false;

            Ip = ((IPEndPoint)(sock.Client.RemoteEndPoint)).Address.ToString();

            // -- Assign the TcpClient to our ClientSocket, to make it start handling events.
            _socket.Accept(sock);
            _lastActive = DateTime.UtcNow;

            // -- Setup the timeout task
            Interval = TimeSpan.FromSeconds(1);
            _taskId = Ip + new Random().Next(2035, 193876957);
            _taskId = TaskScheduler.RegisterTask(_taskId, this);

            Server.RegisterClient(this);

            if (ClassicubePlayer.Database.IsIpBanned(Ip))
            {
                Kick("You are banned.");
            }
        }


        #region Socket Events

        public void SendQueued()
        {
            lock (SendBuffer)
            {
                byte[] allBytes = SendBuffer.GetAllBytes();
                Interlocked.Add(ref Server.SentIncrement, allBytes.Length);
                _socket.Send(allBytes);
                DataAvailable = !BlockChanges.IsEmpty;
            }

            if (BlockChanges.IsEmpty)
                return;

            for (var i = 0; i < 10; i++)
            { // -- If there are queued block changes, write out up to 10 of them at a time to allow other items to have a higher priority..
                if (BlockChanges.TryDequeue(out IIndevPacket packet))
                {
                    packet.Write(SendBuffer);
                }
            }

            if (_disconnectOnSend)
                Shutdown();
        }
        /// <summary>
        /// This gets called when there is data to be sent to the client
        /// </summary>
        private void SendBufferOnDataAdded()
        {
            lock (SendBuffer)
            {
                DataAvailable = true;
            }
        }

        /// <summary>
        /// This is called whenever a client shuts down.
        /// </summary>
        /// <param name="args"></param>
        private void SocketOnDisconnected(SocketDisconnectedArgs args)
        {
            Shutdown();
            Logger.Log(LogType.Info, $"{Ip} Disconnected");
        }

        /// <summary>
        /// This is called whenever data is received from the client
        /// </summary>
        /// <param name="args"></param>
        private void SocketOnDataReceived(DataReceivedArgs args)
        {
            if (!_canReceive)
                return;

            lock (_fk)
            {
                Interlocked.Add(ref Server.ReceivedIncrement, args.Data.Length);
                _receiveBuffer.AddBytes(args.Data); // -- Add the bytes into our receive buffer..
                _lastActive = DateTime.UtcNow;
            }
        }
        #endregion
        /// <summary>
        /// Cross-thread safe packet sending
        /// </summary>
        /// <param name="packet"></param>
		public void SendPacket(IIndevPacket packet)
        {
            lock (SendBuffer)
            {
                packet.Write(SendBuffer);
            }
        }

        /// <summary>
        /// Sends a proper kick to the client, and disconnects them.
        /// </summary>
        /// <param name="reason">The reason for the kick.</param>
        public void Kick(string reason)
        {
            SendPacket(new DisconnectPacket { Slot = 0 });
            Logger.Log(LogType.Info, Verified ? $"{ClientPlayer.LoginName} kicked. ({reason})" : $"{Ip} kicked. ({reason})");
            _disconnectOnSend = true;
            //    Shutdown();
        }

        public void SendHandshake(bool op, string motd = null) {
            var resp = new LoginRequestPacket {
                Username = "",
                ProtocolVersion = 1, // -- Entity ID
                MapSeed = 0,
                Motd = motd ?? Configuration.Settings.General.Motd,
                ServerName = Configuration.Settings.General.Name
            };

           SendPacket(resp);
        }

        /// <summary>
        /// Straight disconnects a client, no warning. Used in final stages of disconnect, or if a client violates protocol.
        /// </summary>
        public void Shutdown()
        {
            _canReceive = false;
            TaskScheduler.UnregisterTask(_taskId);

            if (Verified)
            {
                ClientPlayer.Logout();
                Server.UnregisterClient(this);
            }

            Verified = false;

            if (_socket.IsConnected)
                _socket.Disconnect("");
        }


        private void PopulatePackets()
        {
            _packets = new Dictionary<byte, IIndevPacket> {
                {0, new KeepAlivePacket() },
                {1, new LoginRequestPacket() },
                {2, new HandshakePacket()},
                {3, new ChatMessagePacket() },
                {7, new UseEntityPacket() },
                {10, new PlayerGroundedPacket() },
                {11, new IndevPlayerPositionPacket() },
                {12, new PlayerLookPacket() },
                {13, new PlayerPositionAndLook() },
                {0xFF, new DisconnectPacket() }
            };
        }

        /// <summary>
        /// Handles incoming data
        /// </summary>
        public void Handle()
        {
            if (!_canReceive)
                return;

            var maxIterations = 10;

            while (true)
            { // -- Will continue to try and handle until our buffer is empty, or we need more data.
                if (_receiveBuffer.Length == 0) // -- Check if the buffer is empty
                    break;

                if (maxIterations == 0)
                    break;

                byte opcode = _receiveBuffer.PeekByte(); // -- Peek doesn't remove the byte. Lets us check if we have enough data yet.
                IIndevPacket packet;

                if (!Verified)
                { // -- If this client is unverified, the only packets they're allowed to send are CPE negotiation, and handshake.
                    if (opcode != HandshakePacket.Id && opcode != LoginRequestPacket.Id)
                    {
                        Logger.Log(LogType.Warning, $"Disconnecting {Ip}: Unexpected handshake opcode ({opcode})");
                        Shutdown();
                        return;
                    }
                }

                if (!_packets.TryGetValue(opcode, out packet))
                {
                    Logger.Log(LogType.Warning, $"Received invalid packet from {Ip} ({opcode}), disconnecting.");
                    Kick("Invalid Opcode Received");
                    return;
                }

                if (_receiveBuffer.Length >= packet.PacketLength)
                { // -- Check if we have enough data to read this packet.
                    _receiveBuffer.ReadByte(); // -- Trim off the opcode, the packet is ready to be read.
                    packet.Read(_receiveBuffer); // -- Read the data from the buffer
                    packet.Handle((INetworkClient)this); // -- Handle it.
                }
                else // -- Not enough data, wait for more.
                    break;

                maxIterations--;
            }
        }

        public override void Setup()
        {

        }

        /// <summary>
        /// Check every 1 second to make sure this client hasn't timed out.
        /// </summary>
        public override void Main()
        {
            TimeSpan span = (DateTime.UtcNow - _lastActive);

            if (span.TotalSeconds < 30 && span.TotalSeconds > 5)
            {
                SendPacket(new KeepAlivePacket());
                return;
            }

            //if ((DateTime.UtcNow - _lastActive).TotalSeconds >= 30)
            //{
            //    Kick("&cConnection timed out");
            //}
        }

        public override void Teardown()
        {
            Shutdown();
        }

        public bool HasData()
        {
            return DataAvailable;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void QueueBlockChange(IIndevPacket bcPacket) {
            BlockChanges.Enqueue(bcPacket);
            DataAvailable = true;
        }

        public IMinecraftPlayer GetPlayerInstance() {
            throw new NotImplementedException();
        }
    }
}

