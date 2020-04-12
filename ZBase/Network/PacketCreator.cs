using System;
using ZBase.Common;

namespace ZBase.Network {
    public static class PacketCreator {
        public static IPacket CreateHandshake(byte protocol, byte userType, string name, string motd) {
            return new Handshake {
                ProtocolVersion = protocol,
                Usertype = userType,
                Name = name,
                Motd = motd
            };
        }

        public static IPacket CreateMapChunk(byte[] data, byte percentage, int size) {
            if (size == 1024)
                return new LevelChunk {
                    Data = data,
                    Percent = percentage,
                    Length = (short) size
                }; // -- Account for smaller than 1024 block chunks.

            var newData = new byte[1024];
            Buffer.BlockCopy(data, 0, newData, 0, size);
            data = newData;

            return new LevelChunk {
                Data = data,
                Percent = percentage,
                Length = (short)size
            };
        }
        
        public static IPacket CreateMapFinal(Vector3S mapSize) {
            return new LevelFinalize {
                SizeX = mapSize.X,
                SizeY = mapSize.Y,
                SizeZ = mapSize.Z
            };
        }

        public static IPacket CreateChat(string message) {
            return new Message {
                Text = message,
                PlayerId = 0
            };
        }

        public static IPacket CreateDisconnect(string reason) {
            return new Disconnect { Reason = reason };
        }
    }
}
