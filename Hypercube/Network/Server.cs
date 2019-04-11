using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Sockets;
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

        public static List<Client> Clients;
        public static Client[] RoClients = new Client[0];
        private readonly Thread _handleThread;

        public Server() {
            Interval = TimeSpan.FromSeconds(5);
            Clients = new List<Client>();

            _server = new ServerSocket(Configuration.Settings.Network.ListenPort);
            _server.IncomingClient += ServerOnIncomingClient;
            _server.Listen();

            _handleThread = new Thread(HandleClientData);
            _handleThread.Start();

            TaskScheduler.RegisterTask("Bandwidth", this);
        }

        public void Shutdown() {
            _server.Stop();
            _handleThread.Abort();

            foreach (Client client in RoClients) {
                client.Kick("&eServer shutting down");
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
                Watchdog.Watch("Network IO", "End Loop", false);
                Thread.Sleep(1);
            }
        }

        private void ServerOnIncomingClient(IncomingEventArgs args) {
            // ReSharper disable once ObjectCreationAsStatement
            new Client(args.IncomingClient);
        }

        public static void RegisterClient(Client c) {
            Stopwatch sw = Stopwatch.StartNew();
            lock (Clients) {
                Clients.Add(c);
                RoClients = Clients.ToArray();
                OnlinePlayers = Clients.Count;
            }
            sw.Stop();
        }

        public static void UnregisterClient(Client c) {
            lock (Clients) {
                Clients.Remove(c);
                RoClients = Clients.ToArray();
                OnlinePlayers = Clients.Count;
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
