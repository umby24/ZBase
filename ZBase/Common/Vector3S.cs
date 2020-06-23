namespace ZBase.Common {
    public struct Vector3S {
        public bool Equals(Vector3S other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3S && Equals((Vector3S)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }

        public Vector3S(short x, short y, short z) => (X, Y, Z) = (x, y, z);

        public Vector3S(int x, int y, int z)
        {
            X = (short)x;
            Y = (short)y;
            Z = (short)z;
        }
        public static bool operator ==(Vector3S item1, Vector3S item2) => (item1.X, item1.Y, item1.Z) == (item2.X, item2.Y, item2.Z);

        public static bool operator !=(Vector3S item1, Vector3S item2) => (item1.X, item1.Y, item1.Z) != (item2.X, item2.Y, item2.Z);

        public override string ToString() {
            return $"[Vector: {X}, {Y}, {Z}]";
        }
    }
}
