using System.Linq;
using NUnit.Framework;
using ZBase.Building.BuildModes;
using ZBase.Common;

namespace ZBase.Building.Tests {
    public class Tests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Build3dArrayTest() {
            var givenBrushSize = 5;
            var actual = Brush.Build3dArray(givenBrushSize);
            // -- We expect the array to go from -7 to +7 for each direction. 3d array, 15^3 options.
            Assert.AreEqual(actual.Count, 3375);

            for (var i = -7; i <= 7; i++) {
                var myLocation = new Vector3S(i, -1, -1);
                bool containsLocation = actual.ContainsKey(myLocation);
                Assert.True(containsLocation, $"Expected array to contain [{i}, -1, -1]");
            }
        }
        
        [Test]
        public void BuildVectorArrayTest() {
            var givenSize = 5;
            var result = Brush.BuildVectorArray(givenSize);
            // -- will build an array from -5 to +5, inclusive. 11^3 results in each direction.
            Assert.AreEqual(result.Length, 1331);

            for (var i = -5; i <= 5; i++) {
                var myLocation = new Vector3S(i, 0, 0);
                bool containsLocation = result.Contains(myLocation);
                Assert.True(containsLocation, $"Expected array to contain [{i}, 0, 0]");
            }
            for (var i = -5; i <= 5; i++) {
                var myLocation = new Vector3S(0, i, 0);
                bool containsLocation = result.Contains(myLocation);
                Assert.True(containsLocation, $"Expected array to contain [0, {i}, 0]");
            }
            for (var i = -5; i <= 5; i++) {
                var myLocation = new Vector3S(0, 0, i);
                bool containsLocation = result.Contains(myLocation);
                Assert.True(containsLocation, $"Expected array to contain [0, 0, {i}]");
            }
        }
    }
}