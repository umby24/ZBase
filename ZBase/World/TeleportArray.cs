using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZBase.Common;

namespace ZBase.World {
    public class TeleportArray {
        public List<Teleporter> Portals { get; }
        private byte[] _xArray;
        private byte[] _yArray;
        private byte[] _zArray;

        /// <summary>
        /// Empty constructor for unit tests
        /// </summary>
        protected TeleportArray()
        {

        }

        public TeleportArray(Vector3S mapSize)
        {
            _xArray = new byte[mapSize.X / 8];
            _yArray = new byte[mapSize.Y / 8];
            _zArray = new byte[mapSize.Z / 8];
            Portals = new List<Teleporter>();

        }

        public TeleportArray(List<Teleporter> portals, Vector3S mapSize)
        {
            _xArray = new byte[mapSize.X / 8];
            _yArray = new byte[mapSize.Y / 8];
            _zArray = new byte[mapSize.Z / 8];
            Portals = portals;

            BuildArrays();
        }

        public Teleporter GetPortal(MinecraftLocation location)
        {
            var blockLocation = location.GetAsBlockCoords();

            var xIndex = GetArrayIndex(blockLocation.X);
            var xResult = GetValue(_xArray[xIndex], blockLocation.X) > 0;

            if (!xResult)
                return null;

            var yIndex = GetArrayIndex(blockLocation.Y);
            var yResult = GetValue(_yArray[yIndex], blockLocation.Y) > 0;

            if (!yResult)
                return null;

            var zIndex = GetArrayIndex(blockLocation.Z);
            var zResult = GetValue(_zArray[zIndex], blockLocation.Z) > 0;

            if (!zResult)
                return null;

            // -- Now the expensive part, a lookup.
            var portal = Portals.FirstOrDefault(a => a.InRange(location));
            return portal;
        }

        private void BuildArrays()
        {
            foreach (var teleportal in Portals)
            {
                var startBlock = teleportal.OriginStart.GetAsBlockCoords();
                var endBlock = teleportal.OriginEnd.GetAsBlockCoords();

                _xArray = BuildArray(startBlock.X, endBlock.X, _xArray);
                _yArray = BuildArray(startBlock.Y, endBlock.Y, _yArray);
                _zArray = BuildArray(startBlock.Z, endBlock.Z, _zArray);
            }
        }

        private byte[] BuildArray(int firstCoord, int secondCoord, byte[] array)
        {
            var smallerValue = Math.Min(firstCoord, secondCoord);
            var biggerValue = Math.Max(firstCoord, secondCoord);

            for (var i = smallerValue; i <= biggerValue; i++)
            {
                var index = GetArrayIndex(i);
                array[index] = SetValue(i, array[index]);
            }

            return array;
        }

        protected int GetArrayIndex(int location)
        {
            return (location / 8);
        }

        protected static byte SetValue(int value, byte currentValue)
        {
            byte result = (byte) (currentValue | 1 << (value % 8));
            return result;
        }

        protected static byte GetValue(byte value, int location)
        {
            byte result = (byte)(value & 1 << (location % 8));
            return result;
        }
    }
}
