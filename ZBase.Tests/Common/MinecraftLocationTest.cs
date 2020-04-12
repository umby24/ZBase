using ZBase.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZBase.Tests.Common {
    [TestClass]
    public class MinecraftLocationTest {
        private MinecraftLocation givenMinecraftLocation;
        private Vector3S givenVector;

        [TestInitialize]
        public void Setup()
        {
            givenVector = new Vector3S(100, 100, 100);
            givenMinecraftLocation = new MinecraftLocation(givenVector, 100, 100);
        }

        [TestMethod]
        public void SetBlockCoordsTest()
        {
            givenMinecraftLocation.SetAsBlockCoords(givenVector);

            Assert.AreEqual(3200, givenMinecraftLocation.Location.X);
            Assert.AreEqual(3200, givenMinecraftLocation.Location.Y);
            Assert.AreEqual(3251, givenMinecraftLocation.Location.Z);
        }

        [TestMethod]
        public void GetBlockCoordsTest()
        {
            givenMinecraftLocation.SetAsBlockCoords(givenVector);

            var actualVector = givenMinecraftLocation.GetAsBlockCoords();

            Assert.AreEqual(givenVector, actualVector);
        }

        [TestMethod]
        public void EqualsCheck()
        {
            var givenOtherMinecraftLocation = new MinecraftLocation(givenVector, 100, 100);

            Assert.AreEqual(givenMinecraftLocation, givenOtherMinecraftLocation);
        }

        [TestMethod]
        public void EqualsNegativeCheck()
        {
            var givenOtherMinecraftLocation = new MinecraftLocation(givenVector, 255, 255);

            Assert.AreNotEqual(givenMinecraftLocation, givenOtherMinecraftLocation);
        }
    }
}
