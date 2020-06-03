using ZBase.Common;
using ZBase.World;

namespace ZBase.Network {
    #region Vanilla
    public struct Handshake : IPacket {
        public static byte Id => 0;
        public int PacketLength => 131;
        public byte ProtocolVersion { get; set; }

        public string Name { get;
            set;
        }

        public string Motd { get; set; }
        public byte Usertype { get; set; }

        public void Read(ByteBuffer buf) {
            ProtocolVersion = buf.ReadByte();
            Name = buf.ReadString();
            Motd = buf.ReadString();
            Usertype = buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte(ProtocolVersion);
            buf.WriteString(Name);
            buf.WriteString(Motd);
            buf.WriteByte(Usertype);
            buf.Purge();
        }

        public void Handle(Client c) {
            Logger.Log(LogType.Debug, $"Incoming Name: {Name} mppass: {Motd} unused: {Usertype}");

            if (ProtocolVersion != Server.ProtocolVersion) {
                Logger.Log(LogType.Warning, $"Disconnecting {c.Ip}: Unknown protocol version {ProtocolVersion}");
                c.Shutdown();
                return;
            }
            
            if (!Heartbeat.Verify(c.Ip, Name, Motd)) {
                c.Kick("Failed to verify name"); 
                return;
            }

            c.ClientPlayer = new Player(c) {
                Name = Name,
            };
            
            c.ClientPlayer.Login();
        }
    }

    public struct Ping : IPacket {
        public static byte Id => 1;
        public int PacketLength => 1;

        public void Read(ByteBuffer buf) {

        }
        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.Purge();
        }
        public void Handle(Client c) {
        }
    }

    public struct LevelInit : IPacket {
        public static byte Id => 2;
        public int PacketLength => 1;
        public void Read(ByteBuffer buf) {

        }
        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct LevelChunk : IPacket {
        public static byte Id => 3;
        public int PacketLength => 1028;
        public short Length { get; set; }
        public byte[] Data { get; set; }
        public byte Percent { get; set; }

        public void Read(ByteBuffer buf) {
            Length = buf.ReadShort();
            Data = buf.ReadByteArray();
            Percent = buf.ReadByte();
        }
        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteShort(Length);
            buf.AddBytes(Data);
            buf.WriteByte(Percent);
            buf.Purge();
        }
        public void Handle(Client c) {

        }
    }

    public struct LevelFinalize : IPacket {
        public static byte Id => 4;
        public int PacketLength => 7;
        public short SizeX { get; set; }
        public short SizeY { get; set; }
        public short SizeZ { get; set; }

        public void Read(ByteBuffer buf) {
            SizeX = buf.ReadShort();
            SizeZ = buf.ReadShort();
            SizeY = buf.ReadShort();
        }
        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteShort(SizeX);
            buf.WriteShort(SizeZ);
            buf.WriteShort(SizeY);
            buf.Purge();

        }
        public void Handle(Client c) {

        }
    }

    public struct SetBlock : IPacket {
        public static byte Id => 5;
        public int PacketLength => 9;
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Mode { get; set; }
        public byte Block { get; set; }

        public void Read(ByteBuffer buf) {
            X = buf.ReadShort();
            Z = buf.ReadShort();
            Y = buf.ReadShort();
            Mode = buf.ReadByte();
            Block = buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteShort(X);
            buf.WriteShort(Z);
            buf.WriteShort(Y);
            buf.WriteByte(Mode);
            buf.WriteByte(Block);
            buf.Purge();

        }

        public void Handle(Client c) {
			c.ClientPlayer.HandleBlockPlace (new Vector3S(X, Y, Z), Block, Mode);
        }
    }

    public struct SetBlockServer : IPacket {
        public static byte Id => 6;
        public int PacketLength => 8;
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public byte Block { get; set; }

        public void Read(ByteBuffer buf) {
            X = buf.ReadShort();
            Z = buf.ReadShort();
            Y = buf.ReadShort();
            Block = buf.ReadByte();
        }
        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteShort(X);
            buf.WriteShort(Z);
            buf.WriteShort(Y);
            buf.WriteByte(Block);
            buf.Purge();

        }
        public void Handle(Client c) {

        }
    }

    public struct SpawnPlayer : IPacket {
        public static byte Id => 7;
        public int PacketLength => 74;
        public sbyte PlayerId { get; set; }
        public string PlayerName { get; set; }
        public MinecraftLocation Location { get; set; }

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
            PlayerName = buf.ReadString();
            var temp = new MinecraftLocation();
            var loc = new Vector3S // -- WARNING: This might cause unintended behavior!!
            {
                X = buf.ReadShort(),
                Z = buf.ReadShort(),
                Y = buf.ReadShort()
            };
            temp.SetAsPlayerCoords(loc);
            temp.Rotation = buf.ReadByte();
            temp.Look = buf.ReadByte();
            Location = temp;
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.WriteString(PlayerName);
            buf.WriteShort(Location.Location.X);
            buf.WriteShort(Location.Location.Z);
            buf.WriteShort(Location.Location.Y);
            buf.WriteByte(Location.Rotation);
            buf.WriteByte(Location.Look);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct PlayerTeleport : IPacket {
        public static byte Id => 8;
        public int PacketLength => 10;
        public sbyte PlayerId { get; set; }
        public MinecraftLocation Location { get; set; }
        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte) buf.ReadByte();
            var x = buf.ReadShort();
            var z = buf.ReadShort();
            var y = buf.ReadShort();
            var look = buf.ReadByte();
            var rot = buf.ReadByte();
            Location = new MinecraftLocation(new Vector3S(x, y, z), rot, look);
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.WriteShort(Location.X);
            buf.WriteShort(Location.Z);
            buf.WriteShort(Location.Y);
            buf.WriteByte(Location.Look);
            buf.WriteByte(Location.Rotation);
            buf.Purge();
        }

        public void Handle(Client c) {
			c.ClientPlayer.HandleMove(Location);
        }
    }

    public struct PosAndOrient : IPacket {
        public static byte Id => 9;
        public int PacketLength => 7;
        public sbyte PlayerId { get; set; }
        public sbyte ChangeX { get; set; }
        public sbyte ChangeY { get; set; }
        public sbyte ChangeZ { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
            ChangeX = (sbyte)buf.ReadByte();
            ChangeZ = (sbyte)buf.ReadByte();
            ChangeY = (sbyte)buf.ReadByte();
            Yaw = buf.ReadByte();
            Pitch = buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.WriteShort(ChangeX);
            buf.WriteShort(ChangeZ);
            buf.WriteShort(ChangeY);
            buf.WriteByte(Yaw);
            buf.WriteByte(Pitch);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct PositionUpdate : IPacket {
        public static byte Id => 10;
        public int PacketLength => 5;
        public sbyte PlayerId { get; set; }
        public sbyte ChangeX { get; set; }
        public sbyte ChangeY { get; set; }
        public sbyte ChangeZ { get; set; }

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
            ChangeX = (sbyte)buf.ReadByte();
            ChangeZ = (sbyte)buf.ReadByte();
            ChangeY = (sbyte)buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {

        }
        public void Handle(Client c) {

        }
    }

    public struct OrientationUpdate : IPacket {
        public static byte Id => 11;
        public int PacketLength => 4;
        public sbyte PlayerId { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
            Yaw = buf.ReadByte();
            Pitch = buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.WriteByte(Yaw);
            buf.WriteByte(Pitch);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct DespawnPlayer : IPacket {
        public static byte Id => 12;
        public int PacketLength => 2;
        public sbyte PlayerId;

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct Message : IPacket {
        public static byte Id => 13;
        public int PacketLength => 66;
        public sbyte PlayerId { get; set; }
        public string Text { get; set; }

        public void Read(ByteBuffer buf) {
            PlayerId = (sbyte)buf.ReadByte();
            Text = buf.ReadString();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte((byte)PlayerId);
            buf.WriteString(Text);
            buf.Purge();
        }

        public void Handle(Client c) {

            Chat.HandleIncoming(c, Text);
        }
    }

    public struct Disconnect : IPacket {
        public static byte Id => 14;
        public int PacketLength => 65;
        public string Reason { get; set; }

        public void Read(ByteBuffer buf) {
            Reason = buf.ReadString();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteString(Reason);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }

    public struct UpdateRank : IPacket {
        public static byte Id => 15;
        public int PacketLength => 2;
        public byte Rank { get; set; }

        public void Read(ByteBuffer buf) {
            Rank = buf.ReadByte();
        }

        public void Write(ByteBuffer buf) {
            buf.WriteByte(Id);
            buf.WriteByte(Rank);
            buf.Purge();
        }

        public void Handle(Client c) {

        }
    }
    #endregion
}

