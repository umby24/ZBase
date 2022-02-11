namespace ZBase.Common
{
    public interface IByteBuffer
    {
        int Length { get; }
        byte PeekByte();
        int PeekIntLE();
        byte ReadByte();
        short ReadShort();
        int ReadInt();
        long ReadLong();
        string ReadString();
        byte[] ReadByteArray();
        byte[] ReadByte(int length);
        // -- Write
        void WriteByte(byte value);
        void WriteShort(short value);
        void WriteInt(int value);
        void WriteLong(long value);
        void WriteString(string value);
        void Purge();

        void AddBytes(byte[] data);
        byte[] GetAllBytes();
    }
}
