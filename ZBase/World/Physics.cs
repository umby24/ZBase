using System;
using System.Collections.Concurrent;

namespace ZBase.World {
    public class Physics {
        public byte[] _physicsBitmask;
        public HcMap _Map;

        public Physics(int mapSize, HcMap map) {
            _physicsBitmask = new byte[(mapSize / 8) + 1];
            _Map = map;
        }
        //private int GetPhysicsIndex(int x, int y, int z, int sizeX, int sizeY) {
        //    return (x + y * sizeX + z * sizeX * sizeY) * 1;
        //}

        ///// <summary>
        ///// Adds a set of coordinates to the Physics queue.
        ///// </summary>
        //public void Add(short x, short y, short z) {
        //    var randomGen = new Random();
        //    var mapSize = _Map.GetSize();
        //    // -- Searches around a block that was just placed for physics changes.
        //    for (short ix = -1; ix < 2; ix++) {
        //        for (short iy = -1; iy < 2; iy++) {
        //            for (short iz = -1; iz < 2; iz++) {

        //                if (!_Map.BlockInBounds((short)(x + ix), (short)(y + iy), (short)(z + iz)))
        //                    continue;
                        
        //                int index = GetPhysicsIndex(x + ix, y + iy, z + iz, mapSize.X, mapSize.Y);

        //                if ((_physicsBitmask[index / 8] & (1 << (index % 8))) != 0)
        //                    continue;

        //                //        var blockQueue = _Map.GetBlock((short)(x + ix), (short)(y + iy), (short)(z + iz));

        //                //                        if (blockQueue.Physics <= 0 && string.IsNullOrEmpty(blockQueue.PhysicsPlugin))
        //                //                            continue;

        //                _physicsBitmask[index / 8] = (byte)(_physicsBitmask[index / 8] | (1 << (index % 8)));
        //                // PhysicsQueue.Enqueue(new QueueItem((short)(x + ix), (short)(y + iy), (short)(z + iz), DateTime.UtcNow.AddMilliseconds(blockQueue.PhysicsDelay + randomGen.Next(blockQueue.PhysicsRandom))));
        //            }
        //        }
        //    }
        //}
        
        //void PhysicsOriginalSand(short x, short y, short z) {
        //    if (_Map.GetBlockId(x, y, (short)(z - 1)) == 0)
        //        _Map.MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
        //}

        //void PhysicsD3Sand(short x, short y, short z) {
        //    if (_Map.GetBlockId(x, y, (short)(z - 1)) == 0)
        //        MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
        //    else if (_Map.GetBlockId((short)(x + 1), y, (short)(z - 1)) == 0 && _Map.GetBlockId((short)(x + 1), y, z) == 0)
        //        MoveBlock(x, y, z, (short)(x + 1), y, (short)(z - 1), true, true, 900);
        //    else if (_Map.GetBlockId((short)(x - 1), y, (short)(z - 1)) == 0 && _Map.GetBlockId((short)(x - 1), y, z) == 0)
        //        MoveBlock(x, y, z, (short)(x - 1), y, (short)(z - 1), true, true, 900);
        //    else if (_Map.GetBlockId(x, (short)(y + 1), (short)(z - 1)) == 0 && _Map.GetBlockId(x, (short)(y + 1), z) == 0)
        //        MoveBlock(x, y, z, x, (short)(y + 1), (short)(z - 1), true, true, 900);
        //    else if (_Map.GetBlockId(x, (short)(y - 1), (short)(z - 1)) == 0 && _Map.GetBlockId(x, (short)(y - 1), z) == 0)
        //        MoveBlock(x, y, z, x, (short)(y - 1), (short)(z - 1), true, true, 900);
        //}

        //void PhysicsInfiniteWater(Block physicBlock, short x, short y, short z) {
        //    short playerId = -1;

        //    if (_Map.GetBlockId(x, y, (short)(z - 1)) == 0)
        //        BlockChange(playerId, x, y, (short)(z - 1), physicBlock, _Map.GetBlock(x, y, (short)(z - 1)), true, true, true, 1);
        //    else if (_Map.GetBlockId((short)(x + 1), y, z) == 0)
        //        BlockChange(playerId, (short)(x + 1), y, z, physicBlock, _Map.GetBlock((short)(x + 1), y, z), true, true, true, 1);
        //    else if (_Map.GetBlockId((short)(x - 1), y, z) == 0)
        //        BlockChange(playerId, (short)(x - 1), y, z, physicBlock, _Map.GetBlock((short)(x - 1), y, z), true, true, true, 1);
        //    else if (_Map.GetBlockId(x, (short)(y + 1), z) == 0)
        //        BlockChange(playerId, x, (short)(y + 1), z, physicBlock, _Map.GetBlock(x, (short)(y + 1), z), true, true, true, 1);
        //    else if (_Map.GetBlockId(x, (short)(y - 1), z) == 0)
        //        BlockChange(playerId, x, (short)(y - 1), z, physicBlock, _Map.GetBlock(x, (short)(y - 1), z), true, true, true, 1);
        //}

        void PhysicsFiniteWater(short x, short y, short z) {
            //if (_Map.GetBlockId(x, y, (short)(z - 1)) == 0)
            //    MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 2);
            //else {
            //    var fillArray = new int[1024, 1024];
            //    var fill = new ConcurrentQueue<QueueItem>();
            //    var found = false;

            //    fill.Enqueue(new QueueItem(x, y, z, 1));

            //    while (true) {
            //        QueueItem working;

            //        if (!fill.TryDequeue(out working))
            //            break;

            //        if (GetBlockId(working.X, working.Y, (short)(working.Z - 1)) == 0) {
            //            MoveBlock(x, y, z, working.X, working.Y, (short)(working.Z - 1), true, true, 2);
            //            found = true;
            //        } else {
            //            if (GetBlockId((short)(working.X + 1), working.Y, working.Z) == 0 && fillArray[working.X + 1, working.Y] == 0) {
            //                fillArray[working.X + 1, working.Y] = 1;
            //                fill.Enqueue(new QueueItem((short)(working.X + 1), working.Y, working.Z, 1));
            //            }

            //            if (working.X != 0 && GetBlockId((short)(working.X - 1), working.Y, working.Z) == 0 && fillArray[working.X - 1, working.Y] == 0) {
            //                fillArray[working.X * 1, working.Y] = 1;
            //                fill.Enqueue(new QueueItem((short)(working.X - 1), working.Y, working.Z, 1));
            //            }

            //            if (GetBlockId(working.X, (short)(working.Y + 1), working.Z) == 0 && fillArray[working.X, working.Y + 1] == 0) {
            //                fillArray[working.X, working.Y + 1] = 1;
            //                fill.Enqueue(new QueueItem(working.X, (short)(working.Y + 1), working.Z, 1));
            //            }

            //            if (working.Y != 0 && GetBlockId(working.X, (short)(working.Y - 1), working.Z) == 0 && fillArray[working.X, working.Y - 1] == 0) {
            //                fillArray[working.X, working.Y - 1] = 1;
            //                fill.Enqueue(new QueueItem(working.X, (short)(working.Y - 1), working.Z, 1));
            //            }

            //        }

            //        if (fill.Count > 50000 || found)
            //            fill = new ConcurrentQueue<QueueItem>();
            //    }
            //}
        }

        //void PhysicsSnow(short x, short y, short z) {
        //    if (_Map.GetBlockId(x, y, (short)(z - 1)) == 0 || _Map.GetBlockId(x, y, (short)(z - 1)) == 53)
        //        MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
        //}

    }
}
