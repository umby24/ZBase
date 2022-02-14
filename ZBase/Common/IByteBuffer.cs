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
        float ReadFloat();

        long ReadLong();
        double ReadDouble();
        string ReadString();
        byte[] ReadByteArray();
        byte[] ReadByte(int length);
        // -- Write
        void WriteByte(byte value);
        void WriteShort(short value);
        void WriteInt(int value);
        void WriteFloat(float value);
        void WriteLong(long value);
        void WriteDouble(double value);
        void WriteString(string value);
        void Purge();

        void AddBytes(byte[] data);
        byte[] GetAllBytes();
    }
}
