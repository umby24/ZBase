using ZBase.Common;

namespace ZBase.World {
    public interface IMapProvider {
        string MapName { get; set; }
        string CreatingUser { get; set; }
        string CreatingService { get; set; }
        void CreateNew(Vector3S size, string filePath, string mapName);
        bool Save(string filePath);
        void Load(string filePath);

        Vector3S GetSize();
        void SetSize(Vector3S newSize);

        bool Unload();
        bool Reload();

        void SetBlock(short x, short y, short z, byte type); // -- Sets a block in the providers block array
        byte GetBlock(short x, short y, short z); // -- Gets a block from the providers block array

        void SetBlocks(byte[] blockData); // -- For setting / overwriting the whole map.
        byte[] GetBlocks(); // -- For getting the entire map (Usually for mapsends or the like)

        MinecraftLocation GetSpawn();
        void SetSpawn(MinecraftLocation spawnLocation);
    }
}
