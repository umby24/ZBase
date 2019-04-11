using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Tests.World {
    public class FakeProvider : IMapProvider {
        public string MapName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CreatingUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CreatingService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CreateNew(Vector3S size, string filePath, string mapName) {
            throw new NotImplementedException();
        }

        public byte GetBlock(short x, short y, short z) {
            throw new NotImplementedException();
        }

        public byte[] GetBlocks() {
            throw new NotImplementedException();
        }

        public Vector3S GetSize() {
            throw new NotImplementedException();
        }

        public MinecraftLocation GetSpawn() {
            throw new NotImplementedException();
        }

        public void Load(string filePath) {
            throw new NotImplementedException();
        }

        public bool Reload() {
            throw new NotImplementedException();
        }

        public bool Save(string filePath) {
            throw new NotImplementedException();
        }

        public void SetBlock(short x, short y, short z, byte type) {
            throw new NotImplementedException();
        }

        public void SetBlocks(byte[] blockData) {
            throw new NotImplementedException();
        }

        public void SetSize(Vector3S newSize) {
            throw new NotImplementedException();
        }

        public void SetSpawn(MinecraftLocation spawnLocation) {
            throw new NotImplementedException();
        }

        public bool Unload() {
            throw new NotImplementedException();
        }
    }
}
