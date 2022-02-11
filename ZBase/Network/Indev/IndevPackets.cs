using System;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Network.Indev
{
    public struct KeepAlivePacket : IIndevPacket
    {
        public static byte Id => 0;
        public int PacketLength => 1;

        public void Handle(INetworkClient client)
        {
        }

        public void Read(IByteBuffer client)
        {
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.Purge();
        }
    }

    public struct LoginRequestPacket : IIndevPacket
    {
        public static byte Id => 0x01;
        public int ProtocolVersion { get; set; }
        public string Username { get; set; }
        public long MapSeed { get; set; }
        public string Motd { get; set; }
        public string ServerName { get; set; }
        public int PacketLength => 4 + 8;

        public void Handle(INetworkClient client)
        {
            if (ProtocolVersion > 9)
                Logger.Log(LogType.Info, "Server is outdated.");
            else if (ProtocolVersion < 9)
                Logger.Log(LogType.Info, "Client is outdated.");
            else
            {
                Logger.Log(LogType.Debug, $"Incoming Name: {Username} mppass: {Motd} Protocol: {ProtocolVersion}");

                if (ProtocolVersion != 9) {
                    Logger.Log(LogType.Warning, $"Disconnecting {client.Ip}: Unknown protocol version {ProtocolVersion}");
                    client.Shutdown();
                    return;
                }

                //if (!Heartbeat.Verify(c.Ip, Name, Motd)) {
                //    c.Kick("Failed to verify name");
                //    return;
                //}

                client.ClientPlayer = new IndevPlayer(client) {
                    Name = Username,
                };

                client.ClientPlayer.Login();

                // -- If dead, send update health.
            }

        }

        public void Read(IByteBuffer client)
        {
            ProtocolVersion = client.ReadInt();
            Username = client.ReadString();
            MapSeed = client.ReadLong();
            Motd = client.ReadString();
            ServerName = client.ReadString();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(ProtocolVersion);
            client.WriteString(Username);
            client.WriteLong(MapSeed);
            client.WriteString(Motd);
            client.WriteString(ServerName);
            client.Purge();
        }
    }

    public struct HandshakePacket : IIndevPacket
    {
        public static byte Id => 0x02;
        public string Username { get; set; }
        public int PacketLength => 4 + 4;

        public void Handle(INetworkClient client)
        {
            client.SetName(Username);

            var resp = new HandshakePacket
            {
                Username = "-"
            };
            client.SendPacket(resp);
            
        }

        public void Read(IByteBuffer client)
        {
            Username = client.ReadString();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteString(Username);
            client.Purge();
        }
    }

    public struct HandshakeResponsePacket : IIndevPacket
    {
        public static byte Id => 0x03;
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            throw new NotImplementedException();
        }

        public void Write(IByteBuffer client)
        {
            throw new NotImplementedException();
        }
    }

    public struct ChatMessagePacket : IIndevPacket
    {
        public static byte Id => 0x03;
        public string Message { get; set; }
        public int PacketLength => 3;

        public void Handle(INetworkClient client)
        {
            Chat.HandleIncoming(client, Message);
        }

        public void Read(IByteBuffer client)
        {
            Message = client.ReadString();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteString(Message);
            client.Purge();
        }
    }
    public struct TimeUpdatePacket : IIndevPacket
    {
        public static byte Id => 0x05;
        public long Time { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Time = client.ReadLong();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteLong(Time);
            client.Purge();
        }
    }
    public struct EntityEquipmentPacket : IIndevPacket
    {
        public static byte Id => 0x05;
        public int EntityID { get; set; }
        public short Slot { get; set; }
        public short ItemID { get; set; }
        public short Metadata { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            EntityID = client.ReadInt();
            Slot = client.ReadShort();
            ItemID = client.ReadShort();
            Metadata = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(EntityID);
            client.WriteShort(Slot);
            client.WriteShort(ItemID);
            client.WriteShort(Metadata);
            client.Purge();
        }
    }
    public struct SpawnPositionPacket : IIndevPacket
    {
        public static byte Id => 0x06;
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            X = client.ReadInt();
            Y = client.ReadInt();
            Z = client.ReadInt();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(X);
            client.WriteInt(Y);
            client.WriteInt(Z);
            client.Purge();
        }
    }
    public struct UseEntityPacket : IIndevPacket
    {
        public static byte Id => 0x07;
        public int UserID { get; set; }
        public int TargetID { get; set; }
        public bool LeftClick { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            throw new NotImplementedException();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(UserID);
            client.WriteInt(TargetID);
            client.WriteByte(LeftClick ? (byte)1 : (byte)0);
            client.Purge();
        }
    }
    public struct UpdateHealthPacket : IIndevPacket
    {
        public static byte Id => 0x08;
        public short Health { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Health = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Health);
            client.Purge();
        }
    }
    public struct RespawnPacket : IIndevPacket
    {
        public static byte Id => 0x09;
        public byte Dimension { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Dimension = client.ReadByte();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteByte(Dimension);
            client.Purge();
        }
    }
    public struct PlayerGroundedPacket : IIndevPacket
    {
        public static byte Id => 0x0A;
        public bool OnGround { get; set; }
        public int PacketLength => 2;

        public void Handle(INetworkClient client)
        {
            //throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            OnGround = client.ReadByte() == 1;
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteByte(OnGround ? (byte)1 : (byte)0);
            client.Purge();
        }
    }
    public struct IndevPlayerPositionPacket : IIndevPacket
    {
        public static byte Id => 11;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Stance { get; set; }
        public bool OnGround { get; set; }
        public int PacketLength => 17;

        public void Handle(INetworkClient client)
        {
            //throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            X = (double)client.ReadInt();
            Y = (double)client.ReadInt();
            Stance = client.ReadInt();
            Z = client.ReadInt();
            OnGround = client.ReadByte() > 0;
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteLong((long)X);
            client.WriteLong((long)Y);
            client.WriteLong((long)Z);
            client.WriteLong((long)Stance);
            client.WriteByte(OnGround ? (byte)1 : (byte)0);
            client.Purge();
        }
    }
    public struct PlayerLookPacket : IIndevPacket
    {
        public static byte Id => 0x0C;
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
        public int PacketLength => 9;

        public void Handle(INetworkClient client)
        {
           // throw new NotImplementedException();
        }

        public void Read(IByteBuffer client) {
            Yaw = (float) client.ReadInt();
            Pitch = (float) client.ReadInt();
            OnGround = client.ReadByte() > 0;
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt((int)Yaw);
            client.WriteInt((int)Pitch);
            client.WriteByte(OnGround ? (byte)1 : (byte)0);
            client.Purge();
        }
    }


    public struct PlayerPositionAndLook : IIndevPacket
    {
        public static byte Id => 13;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Stance { get; set; }
        public float Yaw {get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
        public int PacketLength => 41;

        public void Handle(INetworkClient client)
        {
           // throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        { // -- TODO: Note; This packet may be a different order depending on clientbound or server bound.
            X = client.ReadInt();
            Y = client.ReadInt();
            Z = client.ReadInt();
            Stance = client.ReadInt();
            Yaw = (float)client.ReadInt();
            Pitch = (float)client.ReadInt();
            OnGround = client.ReadByte() > 0;
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteLong((long)X);
            client.WriteLong((long)Y);
            client.WriteLong((long)Z);
            client.WriteLong((long)Stance);
            client.WriteInt((int)Yaw);
            client.WriteInt((int)Pitch);
            client.WriteByte(OnGround ? (byte)1 : (byte)0);
            client.Purge();
        }
    }
    public struct SetPlayerPositionPacket : IIndevPacket
    {
        public static byte Id => 0x0D;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Stance { get; set; }
        public float Yaw {get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            X = client.ReadLong();
            Stance = client.ReadLong();
            Y = client.ReadLong();
            Z = client.ReadLong();
            Yaw = (float)client.ReadInt();
            Pitch = (float)client.ReadInt();
            OnGround = client.ReadByte() > 0;
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteLong((long)X);
            client.WriteLong((long)Stance);
            client.WriteLong((long)Y);
            client.WriteLong((long)Z);
            client.WriteInt((int)Yaw);
            client.WriteInt((int)Pitch);
            client.WriteByte(OnGround ? (byte)1 : (byte)0);
            client.Purge();
        }
    }
    public struct PlayerDiggingPacket : IIndevPacket
    {
        public enum Action {
            StartDigging = 0,
            StopDigging = 2,
            DropItem = 4
        }
        
        public static byte Id => 0x0E;
        public Action PlayerAction;
        public int X { get; set; }
        public sbyte Y { get; set; }
        public int Z { get; set; }
        public sbyte Blockface { get; set; }
        
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client) {
            PlayerAction = (Action)client.ReadByte();
            X = client.ReadInt();
            Y = (sbyte)client.ReadByte();
            Z = client.ReadInt();
            Blockface = (sbyte)client.ReadByte();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteByte((byte)PlayerAction);
            client.WriteInt(X);
            client.WriteByte((byte)Y);
            client.WriteInt(Z);
            client.WriteByte((byte)Blockface);
        }
    }
    /// <summary>
    /// Sent when the player interacts with a block (generally via right clicking).
    /// This is also used for items that don't interact with blocks (i.e. food) with the coordinates set to -1.
    /// </summary>
    public struct PlayerBlockPlacementPacket : IIndevPacket
    {
        public static byte Id => 0x0F;
        public int X { get; set; }
        public sbyte Y { get; set; }
        public int Z { get; set; }
        public sbyte Blockface { get; set; }
        public short ItemID { get; set; }
        public sbyte? Amount { get; set; }
        public short? Metadata { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client) {
            X = client.ReadInt();
            Y = (sbyte)client.ReadByte();
            Z = client.ReadInt();
            Blockface = (sbyte) client.ReadByte();
            ItemID = client.ReadShort();
            
            if (ItemID != -1) {
                Amount = (sbyte)client.ReadByte();
                Metadata = client.ReadShort();
            }
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(X);
            client.WriteByte((byte)Y);
            client.WriteInt(Z);
            client.WriteByte((byte)Blockface);
            client.WriteShort(ItemID);
            
            if (ItemID != -1) {
                client.WriteByte((byte)Amount.Value);
                client.WriteShort(Metadata.Value);
            }
        }
    }
    public struct ChangeHeldItemPacket : IIndevPacket
    {
        public static byte Id => 0x10;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client) {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }
    public struct SetGameMode : IIndevPacket // -- DIFF
    {
        public static byte Id => 0x11;
        public int EntityId { get; set; }
        public byte GameMode { get; set; }
        public byte IsOp { get; set; }
        public byte Revive { get; set; }

        public int PacketLength => 7;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            EntityId = client.ReadInt();
            GameMode = client.ReadByte();
            IsOp = client.ReadByte();
            Revive = client.ReadByte();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(EntityId);
            client.WriteByte(GameMode); 
            client.WriteByte(IsOp);
            client.WriteByte(Revive);
            client.Purge();
        }
    }

    public struct SetCreativeInventory : IIndevPacket // -- DIFF
    {
        public static byte Id => 0x12;
        public int CurrentItem { get; set; }
        public int ItemId { get; set; }
        public int PacketLength => 8;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            CurrentItem = client.ReadInt();
            ItemId = client.ReadInt();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(CurrentItem);
            client.WriteInt(ItemId);
            client.Purge();
        }
    }

    //public struct PlayerActionPacket : IIndevPacket
    //{
    //    public static byte Id => 0x13;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    public struct SpawnPlayerPacket : IIndevPacket
    {
        public static byte Id => 0x14;
        public int EntityId { get; set; }
        public string Name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public byte rotation { get; set; }
        public byte pitch { get; set; }
        public int currentItem { get; set; }
        public int PacketLength => 28;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            EntityId = client.ReadInt();
            Name = client.ReadString();
            x = client.ReadInt();
            y = client.ReadInt();
            z = client.ReadInt();
            rotation = client.ReadByte();
            pitch = client.ReadByte();
            currentItem = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(EntityId);
            client.WriteString(Name);
            client.WriteInt(x);
            client.WriteInt(y);
            client.WriteInt(z);
            client.WriteByte(rotation);
            client.WriteByte(pitch);
            client.WriteShort((short)currentItem);
            client.Purge();
        }
    }

    public struct SpawnItemPacket : IIndevPacket
    {
        public static byte Id => 0x15; // 21
        public short Slot { get; set; }
        public int PacketLength => 24;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct CollectItemPacket : IIndevPacket
    {
        public static byte Id => 0x16;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct SpawnGenericEntityPacket : IIndevPacket
    {
        public static byte Id => 0x17;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct SpawnMobPacket : IIndevPacket
    {
        public static byte Id => 0x18;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct SpawnPaintingPacket : IIndevPacket
    {
        public static byte Id => 0x19;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct EntityVelocityPacket : IIndevPacket
    {
        public static byte Id => 0x1C;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct DestroyEntityPacket : IIndevPacket
    {
        public static byte Id => 0x1D;
        public int EntityID;
        public int PacketLength => 5;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            // -- Clientbound only
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(EntityID);
            client.Purge();
        }
    }

    public struct EntityRelativeMovePacket : IIndevPacket
    {
        public static byte Id => 0x1F;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct EntityLookPacket : IIndevPacket
    {
        public static byte Id => 0x20;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct EntityLookAndRelativeMovePacket : IIndevPacket
    {
        public static byte Id => 0x21;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct EntityTeleportPacket : IIndevPacket
    {
        public static byte Id => 0x22;
        public int EntityID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public sbyte Yaw, Pitch;
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            // -- Clientbound only
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(EntityID);
            client.WriteInt(X);
            client.WriteInt(Y);
            client.WriteInt(Z);
            client.WriteByte((byte)Yaw);
            client.WriteByte((byte)Pitch);
            client.Purge();
        }
    }

    public struct EntityStatusPacket : IIndevPacket
    {
        public static byte Id => 0x26;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    //public struct AttachEntityPacket : IIndevPacket
    //{
    //    public static byte Id => 0x27;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    public struct EntityMetadataPacket : IIndevPacket
    {
        public static byte Id => 0x28;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct LevelData : IIndevPacket // -- DIFF
    {
        public static byte Id => 0x32;
        public int MapSize { get; set; }
        public int MetaSize { get; set; }
        public byte[] MapData { get; set; }
        public byte[] MetaData { get; set; }

        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            // -- Clientbound only
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(MapSize);
            client.WriteInt(MetaSize);
            client.WriteInt(100);
            client.WriteInt(MapSize);
            client.AddBytes(MapData);
            client.WriteInt(MetaSize);
            client.AddBytes(MetaData);

            client.Purge();
        }
    }

    public struct LevelFinalize : IIndevPacket // -- DIFF
    {
        public static byte Id => 0x33;
        public int LevelType { get; set; }
        public int LevelSize { get; set; }
        public int LevelShape { get; set; }
        public int LevelTheme { get; set; }
        public int WorldTime { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        public int PacketLength => 20;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            // Clientbound only
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteByte((byte)LevelType);
            client.WriteByte((byte)LevelSize);
            client.WriteByte((byte)(LevelShape));
            client.WriteByte((byte)LevelTheme);
            client.WriteInt(Width);
            client.WriteInt(Height);
            client.WriteInt(Depth);
            client.WriteInt(WorldTime);
            client.Purge();
        }
    }

    public struct BulkBlockChangePacket : IIndevPacket
    {
        public static byte Id => 0x34;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct BlockChangePacket : IIndevPacket
    {
        public static byte Id => 0x35;
        public int X { get; set; }
        public sbyte Y { get; set; }
        public int Z { get; set; }
        public sbyte BlockID { get; set; }
        public sbyte Metadata { get; set; }
        public int PacketLength => 12;

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteInt(X);
            client.WriteByte((byte)Y);
            client.WriteInt(Z);
            client.WriteByte((byte)BlockID);
            client.WriteByte((byte)Metadata);
            client.Purge();
        }
    }

    //public struct BlockActionPacket : IIndevPacket
    //{
    //    public static byte Id => 0x36;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    //public struct ExplosionPacket : IIndevPacket
    //{
    //    public static byte Id => 0x3C;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    public struct SoundEffectPacket : IIndevPacket
    {
        public static byte Id => 0x3D;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    //public struct EnvironmentStatePacket : IIndevPacket
    //{
    //    public static byte Id => 0x46;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    public struct SetScore : IIndevPacket // -- DIFF
    {
        public static byte Id => 0x47;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct OpenWindowPacket : IIndevPacket
    {
        public static byte Id => 0x64;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct CloseWindowPacket : IIndevPacket
    {
        public static byte Id => 0x65;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct ClickWindowPacket : IIndevPacket
    {
        public static byte Id => 0x66;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct SetSlotPacket : IIndevPacket
    {
        public static byte Id => 0x67;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct WindowItemsPacket : IIndevPacket
    {
        public static byte Id => 0x68;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct UpdateProgressPacket : IIndevPacket
    {
        public static byte Id => 0x69;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct TransactionStatusPacket : IIndevPacket
    {
        public static byte Id => 0x6A;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct UpdateSignPacket : IIndevPacket
    {
        public static byte Id => 0x82;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    //public struct MapDataPacket : IIndevPacket
    //{
    //    public static byte Id => 0x83;
    //    public short Slot { get; set; }
    //    public int PacketLength => throw new NotImplementedException();

    //    public void Handle(INetworkClient client)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Read(IByteBuffer client)
    //    {
    //        Slot = client.ReadShort();
    //    }

    //    public void Write(IByteBuffer client)
    //    {
    //        client.WriteByte(Id);
    //        client.WriteShort(Slot);
    //        client.Purge();
    //    }
    //}

    public struct UpdateStatisticPacket : IIndevPacket
    {
        public static byte Id => 0xC8;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }

    public struct DisconnectPacket : IIndevPacket
    {
        public static byte Id => 0xFF;
        public short Slot { get; set; }
        public int PacketLength => throw new NotImplementedException();

        public void Handle(INetworkClient client)
        {
            throw new NotImplementedException();
        }

        public void Read(IByteBuffer client)
        {
            Slot = client.ReadShort();
        }

        public void Write(IByteBuffer client)
        {
            client.WriteByte(Id);
            client.WriteShort(Slot);
            client.Purge();
        }
    }
}

