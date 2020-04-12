namespace ZBase.Common {
    public struct MinecraftLocation {
        public short X => Location.X;
        public short Y => Location.Y;
        public short Z => Location.Z;
        public Vector3S Location { get; private set; }
        public byte Rotation { get; set; }
        public byte Look { get; set; }

        public MinecraftLocation(Vector3S location, byte rot, byte look) => (Location, Rotation, Look) = (location, rot, look);

        /// <summary>
        /// Sets this location based on block coordinates.
        /// </summary>
        /// <param name="blockCoords"></param>
        public void SetAsBlockCoords(Vector3S blockCoords)
        {
            Location = new Vector3S
            {
                X = (short)(blockCoords.X * 32),
                Y = (short)(blockCoords.Y * 32),
                Z = (short)((blockCoords.Z * 32) + 51)
            };
        }
        public void SetAsPlayerCoords(Vector3S playerCoords)
        {
            Location = new Vector3S
            {
                X = playerCoords.X,
                Y = playerCoords.Y,
                Z = playerCoords.Z
            };
        }
        public Vector3S GetAsBlockCoords()
        {
            return new Vector3S
            {
                X = (short)(Location.X / 32),
                Y = (short)(Location.Y / 32),
                Z = (short)((Location.Z / 32) - 1)
            };
        }

        public static bool operator ==(MinecraftLocation first, MinecraftLocation second) => (first.Location, first.Rotation, first.Look) == (second.Location, second.Rotation, second.Look);
        public static bool operator !=(MinecraftLocation first, MinecraftLocation second) => (first.Location, first.Rotation, first.Look) != (second.Location, second.Rotation, second.Look);
        public bool Equals(MinecraftLocation other) => this == other;
        public override bool Equals(object obj) => (obj is MinecraftLocation ml) ? this == ml : false;
        public override int GetHashCode() => Location.GetHashCode() *397 ^ Look.GetHashCode() * 397 ^ Rotation.GetHashCode();
    }
}
