using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using ZBase.World;

namespace ZBase.Tests.World {
    public class TeleportArrayTests : TeleportArray {
        [Test]
        public void SetValueTest()
        {
            var expected = new Dictionary<int, int>
            {
                // -- Given, Expected.
                {57, 2},
                {58, 6},
                {59, 14},
                {60, 30 },
                {61, 62 },
                {62, 126 },
                {63, 254 },
                {64, 255 }
            };

            byte actual = 0;

            foreach (var pair in expected)
            {
                actual = TeleportArray.SetValue(pair.Key, actual);
                Debug.WriteLine($"{pair.Key} : {actual}");
                Assert.AreEqual(pair.Value, actual);
            }
        }

        [Test]
        public void GetValueTest()
        {
            var expectedZeros = new Dictionary<int, int>
            {
                {57, 0},
                {58, 2},
                {59, 6},
                {60, 14 },
                {61, 30 },
                {62, 62 },
                {63, 126 },
                {64, 254 }
            };

            var expectedOnes = new Dictionary<int, int>
            {
                {57, 2},
                {58, 6},
                {59, 14},
                {60, 30 },
                {61, 62 },
                {62, 126 },
                {63, 254 },
                {64, 255 }
            };

            foreach (var pair in expectedOnes)
            {
                var actual = TeleportArray.GetValue((byte)pair.Value, pair.Key);
                Assert.GreaterOrEqual(actual, 1);
            }

            foreach (var pair in expectedZeros)
            {
                var actual = TeleportArray.GetValue((byte)pair.Value, pair.Key);
                Assert.AreEqual(0, actual);
            }
            // -- for 255, any value you throw at it should return '1'
            // -- for 254, 64 should return 0, anything else should return '1'.
        }
    }
}
