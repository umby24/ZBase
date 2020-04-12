using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Sockets;
using Sockets.EventArgs;
using ZBase.Common;

namespace ZBase.Network {
    public class NetworkClient : TaskItem, INetworkClient {

        private readonly ClientSocket _baseSocket;
        private readonly ByteBuffer _receiveBuffer, _sendBuffer; // -- Raw network data to be processed.
        private readonly ConcurrentQueue<IPacket> _eventQueue; // -- packets to be processed

        public NetworkClient() {
            _receiveBuffer = new ByteBuffer();
            _sendBuffer = new ByteBuffer();
            _baseSocket = new ClientSocket();

            // -- Event Registration
            _sendBuffer.DataAdded += SendBufferOnDataAdded;
            _baseSocket.DataReceived += BaseSocketOnDataReceived;
            _baseSocket.Disconnected += BaseSocketOnDisconnected;
        }

        public void AcceptClient(TcpClient incomingClient) {
            _baseSocket.Accept(incomingClient);
        }

        private void BaseSocketOnDisconnected(SocketDisconnectedArgs args) {
            throw new NotImplementedException();
        }

        private void BaseSocketOnDataReceived(DataReceivedArgs args) {
            lock (_receiveBuffer) {
                _receiveBuffer.AddBytes(args.Data);
            }
        }

        private void SendBufferOnDataAdded() {
            throw new NotImplementedException();
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
    }
}
