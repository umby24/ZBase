using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ManagedSockets;
using Sockets.EventArgs;
using ZBase.Common;

namespace ZBase.Network {
    public class Server : TaskItem {
        public const int ProtocolVersion = 7;
        public static int OnlinePlayers;
        public static long BytesSent;
        public static long BytesReceived;
        internal static int SentIncrement;
        internal static int ReceivedIncrement;

        private readonly ServerSocket _server;
        private readonly ServerSocket _indevServer;

        public static List<INetworkClient> IClients;
        public static List<Client> Clients;
        public static INetworkClient[] IROClients = new INetworkClient[0];

        public static Client[] RoClients = new Client[0];
        private readonly Thread _handleThread;

        public Server() {
            Interval = TimeSpan.FromSeconds(5);
            Clients = new List<Client>();
            IClients = new List<INetworkClient>();

            _server = new ServerSocket(Configuration.Settings.Network.ListenPort);
            _server.IncomingClient += ServerOnIncomingClient;
            _server.Listen();

            _indevServer = new ServerSocket(Configuration.Settings.Network.ListenPort + 1);
            _indevServer.IncomingClient += _indevServer_IncomingClient;
            _indevServer.Listen();
            Logger.Log(LogType.Info, "Waiting for Indev Clients on " + (Configuration.Settings.Network.ListenPort + 1));
            _handleThread = new Thread(HandleClientData);
            _handleThread.Start();

            TaskScheduler.RegisterTask("Bandwidth", this);
        }

        private void _indevServer_IncomingClient(IncomingEventArgs args)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new IndevClient(args.IncomingClient);
        }

        public void Shutdown() {
            _server.Stop();
            _handleThread.Abort();

            foreach (Client client in RoClients) {
                client.Kick("&eServer shutting down");
            }

            foreach(INetworkClient c in IROClients)
            {
                c.Kick("&eServer shutting down.");
            }
        }

        public void HandleClientData() {
            while (true) {
                Watchdog.Watch("Network IO", "Begin Loop", false);
                foreach (Client roClient in RoClients) {
                    if (roClient.DataAvailable) {
                        roClient.SendQueued();
                    }

                    roClient.Handle();
                }

                foreach (INetworkClient roClient in IROClients)
                {
                    if (roClient.HasData())
                    {
                        roClient.SendQueued();
                    }

                    try {
                        roClient.Handle();
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error packet handling: {ex.Message} at {ex.StackTrace.ToString()}");
                    }
                    
                }

                Watchdog.Watch("Network IO", "End Loop", false);
                Thread.Sleep(1);
            }
            Console.WriteLine("YO WTF");
        }

        private void ServerOnIncomingClient(IncomingEventArgs args) {
            // ReSharper disable once ObjectCreationAsStatement
            new Client(args.IncomingClient);
        }


        public static void RegisterClient(INetworkClient c)
        {
            lock (IClients)
            {
                IClients.Add(c);
                IROClients = IClients.ToArray();
                OnlinePlayers = Clients.Count + IClients.Count;
            }
        }

        public static void RegisterClient(Client c) {
            lock (Clients) {
                Clients.Add(c);
                RoClients = Clients.ToArray();
                OnlinePlayers = Clients.Count + IClients.Count;
            }
        }

        public static void UnregisterClient(Client c) {
            lock (Clients) {
                Clients.Remove(c);
                RoClients = Clients.ToArray();
                OnlinePlayers = Clients.Count;
            }
        }

        public static void UnregisterClient(INetworkClient c)
        {
            lock (IClients)
            {
                IClients.Remove(c);
                IROClients = IClients.ToArray();
                OnlinePlayers = Clients.Count + IClients.Count;
            }
        }

        /// <summary>
        /// Send a packet to all connected clients.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="extension"></param>
        /// <param name="extVersion"></param>
        public static void SendToAll(IPacket packet, string extension = null, int extVersion = 0) {
          //  lock (Clients) {
                foreach (Client client in RoClients) {
                    if (extension != null) {
                        continue;
                    }

					client.SendPacket (packet);
                }
          //  }
        }

        public static void SendAllExcept(IPacket packet, Client not) {
        //    lock (Clients) {
                foreach (Client client in RoClients) {
                    if (client == not)
                        continue;
					client.SendPacket (packet);
                }
          //  }
        }

        public override void Setup() {
            
        }

        public override void Main() {
            // -- Sets up for KB/s stats.
            BytesReceived = ReceivedIncrement/1024;
            BytesSent = SentIncrement/1024;
            ReceivedIncrement = 0;
            SentIncrement = 0;
            Logger.Log(LogType.Debug, "Recv: " + BytesReceived + " KB/s, Sent: " + BytesSent + " KB/s.");
        }

        public override void Teardown() {
            
        }
    }
}
