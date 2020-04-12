using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Tests.World {
    public class TeleporterTests {
        [Test]
        public void InRangeTest()
        {
            var startVector = new Vector3S(64, 64, 64);
            var endVector = new Vector3S(75, 64, 66);
            var givenVector = new Vector3S(65, 64, 65);

            var startLocation = new MinecraftLocation();
            startLocation.SetAsBlockCoords(startVector);
            
            var endLocation = new MinecraftLocation();
            endLocation.SetAsBlockCoords(endVector);

            var givenLocation = new MinecraftLocation();
            givenLocation.SetAsBlockCoords(givenVector);

            var underTest = new Teleporter(startLocation, endLocation, endLocation, "test", "test");
            var result = underTest.InRange(givenLocation);

            Assert.True(result);
        }

        [Test]
        public void InRangeExactTest()
        {
            var startVector = new Vector3S(64, 64, 64);
            var endVector = new Vector3S(65, 64, 64);
            var givenVector = new Vector3S(65, 64, 64);

            var startLocation = new MinecraftLocation();
            startLocation.SetAsBlockCoords(startVector);

            var endLocation = new MinecraftLocation();
            endLocation.SetAsBlockCoords(endVector);

            var givenLocation = new MinecraftLocation();
            givenLocation.SetAsBlockCoords(givenVector);

            var underTest = new Teleporter(startLocation, endLocation, endLocation, "test", "test");
            var result = underTest.InRange(givenLocation);

            Assert.True(result);
        }

        [Test]
        public void InRangeNegativeTest()
        {
            var startVector = new Vector3S(64, 64, 64);
            var endVector = new Vector3S(75, 64, 66);
            var givenVector = new Vector3S(76, 64, 65);

            var startLocation = new MinecraftLocation();
            startLocation.SetAsBlockCoords(startVector);

            var endLocation = new MinecraftLocation();
            endLocation.SetAsBlockCoords(endVector);

            var givenLocation = new MinecraftLocation();
            givenLocation.SetAsBlockCoords(givenVector);

            var underTest = new Teleporter(startLocation, endLocation, endLocation, "test", "test");
            var result = underTest.InRange(givenLocation);

            Assert.False(result);
        }
    }
}
