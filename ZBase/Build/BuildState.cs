using System.Collections.Generic;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Build {
    /// <summary>
    /// Buildstate provides a static state for each player in which temporary block changes and variables can be stored and used by commands
    /// This allows ease of creation of commands such as /box (cuboid) and other multi-state plugins
    /// And allows any block changes made by the player while in a state to be reversed without changing the map directly.
    /// </summary>
    public class BuildState {
        /// <summary>
        ///  Constant size of the block resend queue
        /// </summary>
        public const int MaxResendSize = 1000;
        public List<string> SItems; // -- String variable items
        public List<float> FItems; // -- Float variable items
        public List<int> Items; // -- Integer variable items
        public List<MinecraftLocation> CoordItems; // -- Coordinate variable items
        public List<Vector3S> Blocks; // -- Blocks which were modified and need to be resent to the player.
        public BuildMode CurrentMode { get; set; }

        public BuildState()
        {
            SItems = new List<string>();
            FItems = new List<float>();
            Items = new List<int>();
            CoordItems = new List<MinecraftLocation>();
            Blocks = new List<Vector3S>();
        }

        public string GetString(int index)
        {
            return SItems.Count >= index + 1 ? SItems[index] : null;
        }

        public float GetFloat(int index)
        {
            return FItems[index];
        }

        public int GetInt(int index)
        {
            return Items[index];
        }

        public MinecraftLocation GetCoord(int index)
        {
            return CoordItems[index];
        }

        // -- Set functions
        public void Set(string value, int index)
        {
            if ((index + 1) > SItems.Count)
            {
                for (var i = 0; i < (index + 1); i++)
                    SItems.Add(null);
            }

            SItems.Insert(index, value);
        }

        public void Set(float value, int index)
        {
            if ((index + 1) > FItems.Count)
            {
                for (var i = 0; i < (index + 1); i++)
                    FItems.Add(0.0f);
            }

            FItems.Insert(index, value);
        }

        public void Set(int value, int index)
        {
            if ((index + 1) > Items.Count)
            {
                for (var i = 0; i < (index + 1); i++)
                    Items.Add(0);
            }

            Items[index] = value;
        }

        public void SetCoord(MinecraftLocation coord, int index)
        {
            if ((index + 1) > CoordItems.Count)
            {
                for (var i = 0; i < (index + 1); i++)
                    CoordItems.Add(new MinecraftLocation());
            }
            CoordItems.Insert(index, coord);
        }

        public void SetCoord(short x, short y, short z, int index)
        {
            if ((index + 1) > CoordItems.Count)
            {
                for (var i = 0; i < (index + 1); i++)
                    CoordItems.Add(new MinecraftLocation());
            }

            var myCoord = new Vector3S { X = x, Y = y, Z = z };
            var myLocation = new MinecraftLocation();
            myLocation.SetAsBlockCoords(myCoord);
            CoordItems.Insert(index, myLocation);
        }

        public void AddBlock(short x, short y, short z)
        {
            var thisPoint = new Vector3S { X = x, Y = y, Z = z };

            if (Blocks.Contains(thisPoint))
                return;

            if (Blocks.Count < MaxResendSize)
                Blocks.Add(thisPoint);
            else
            {
                Blocks.RemoveAt(0);
                Blocks.Add(thisPoint);
            }
        }

        public void ResendBlocks(Client client)
        {
            foreach (Vector3S point in Blocks)
            {
                // -- Queue up a block change for this one point for this specific player.
                client.ClientPlayer.BounceBlock(point);
            }

            Blocks.Clear();
        }
    }
}