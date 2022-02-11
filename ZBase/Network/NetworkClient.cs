using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ManagedSockets;
using ManagedSockets.EventArgs;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Network {
    public class NetworkClient : TaskItem, INetworkClient {

        private readonly ClientSocket _baseSocket;
        private readonly ByteBuffer _receiveBuffer, _sendBuffer; // -- Raw network data to be processed.
        private readonly ConcurrentQueue<IPacket> _eventQueue; // -- packets to be processed
        public string Ip { get; set; }
        public string Name { get; set; }
        public bool Verified { get; set; }
        public IMinecraftPlayer ClientPlayer { get; set; }

        private bool _dataAvailable;

        public NetworkClient(TcpClient sock) {
            _receiveBuffer = new ByteBuffer();
            _sendBuffer = new ByteBuffer();
            _baseSocket = new ClientSocket();

            // -- Event Registration
            _sendBuffer.DataAdded += SendBufferOnDataAdded;
            _baseSocket.DataReceived += BaseSocketOnDataReceived;
            _baseSocket.Disconnected += BaseSocketOnDisconnected;
            
            Ip = _baseSocket.Endpoint.Address.ToString();
            _dataAvailable = false;
        }

        private void BaseSocketOnDisconnected(SocketDisconnectedArgs args) {
            Shutdown();
            Logger.Log(LogType.Info, $"{Ip} Disconnected");
        }

        private void BaseSocketOnDataReceived(DataReceivedArgs args) {
            lock (_receiveBuffer) {
                _receiveBuffer.AddBytes(args.Data);
            }
        }

        private void SendBufferOnDataAdded() {
            lock (_sendBuffer) {
                _dataAvailable = true;
            }
        }


        public void Kick(string reason) {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        public void ReceiveLoop() {
            throw new NotImplementedException();
        }

        public void SendLoop() {
            lock (_sendBuffer) {
                byte[] currentBuffer = _sendBuffer.GetAllBytes();
                _baseSocket.Send(currentBuffer);
            }
        }

        public void ProcessEvents() {
            lock (_receiveBuffer) {
                if (_receiveBuffer.Length == 0)
                    return;

            }
        }

        public override void Setup() {
            
        }

        public override void Main() {
          
        }

        public override void Teardown() {
          
        }

        public bool HasData() => _dataAvailable;

        public void SendQueued()
        {
            throw new NotImplementedException();
        }

        public void Handle()
        {
            throw new NotImplementedException();
        }

        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        public void SendPacket(IIndevPacket packet)
        {
            throw new NotImplementedException();
        }

        public void QueueBlockChange(IIndevPacket bcPacket) {
            throw new NotImplementedException();
        }

        public void SendHandshake(bool op, string motd = null) {
            throw new NotImplementedException();
        }

        public IMinecraftPlayer GetPlayerInstance() {
            throw new NotImplementedException();
        }
    }
}
