using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using ZBase.Fills;

namespace ZBase.Tests.Fills {
    public class FlyingIslandsTests {
        [Test]
        public void TestRandom() {
            var actual = FlyingIslands.Random(0, 0, 115);
            Assert.AreEqual(0.86211988654577, actual);

            var meh = FlyingIslands.Random(0, 0, -229);
            var exp = 0.8302870738261845;

            Assert.AreEqual(exp, meh);

            var meh3 = FlyingIslands.Random(16, 0, -229);
            var exp3 = 0.75946614789427258;

            Assert.AreEqual(exp3, meh3);

            var meh4 = FlyingIslands.Random(16, 16, -229);
            var exp4 = 0.79468854591686977;

            Assert.AreEqual(exp4, meh4);
        }

        [Test]
        public void TestQuantize() {
            var expected = new float[] { 0f, 0f };
            var secondExpected = new float[] { 0f, 1f };
            var thirdExpected = new float[] { 0f, 0f };
            var actual = FlyingIslands.Quantize(0, 0, 1);
            var secondActual = FlyingIslands.Quantize(0, 1, 1);
            var thirdActual = FlyingIslands.Quantize(1, 5, 8);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(secondExpected, secondActual);
            Assert.AreEqual(thirdExpected, thirdActual);
        }

        [Test]
        public void TestGenerate2dPointArray_inclusive() {
            const int givenStart = 0;
            const int givenEnd = 15;
            const int expectedLength = 256;
            var expectedStartPoint = new Vector2S(givenStart, givenStart);
            var expectedEndPoint = new Vector2S(givenEnd, givenEnd);

            List<Vector2S> actualArray = FlyingIslands.Generate2dPointArray(givenStart, givenEnd, true);
            Assert.AreEqual(expectedLength, actualArray.Count);
            Assert.True(actualArray.Contains(expectedStartPoint));
            Assert.True(actualArray.Contains(expectedEndPoint));
        }

        [Test]
        public void TestGenerate2dPointArray_exclusive()
        {
            const int givenStart = 0;
            const int givenEnd = 15;
            const int expectedLength = 225;
            var expectedStartPoint = new Vector2S(givenStart, givenStart);
            var expectedEndPoint = new Vector2S(givenEnd-1, givenEnd-1);

            List<Vector2S> actualArray = FlyingIslands.Generate2dPointArray(givenStart, givenEnd, false);
            Assert.AreEqual(expectedLength, actualArray.Count);
            Assert.True(actualArray.Contains(expectedStartPoint));
            Assert.True(actualArray.Contains(expectedEndPoint));
        }

        private Dictionary<Vector2S, double> doubleHeightMapOg(Dictionary<Vector2S, double> map, int size) {
            for (var ix = size; ix >= 0; ix--) {
                for (var iy = size; iy >= 0; iy--) {
                    var doublePoint = new Vector2S(ix, iy);

                    if (!map.ContainsKey(doublePoint))
                        map.Add(doublePoint, map[new Vector2S(ix, iy)]);
                }
            }

            return map;
        }

        private void AssertDictionaryEqual(Dictionary<Vector2S, double> first, Dictionary<Vector2S, double> second) {
            Assert.AreEqual(first.Count, second.Count);
            foreach (KeyValuePair<Vector2S, double> keyValuePair in first) {
                Assert.True(second.ContainsKey(keyValuePair.Key));
                Assert.AreEqual(keyValuePair.Value, second[keyValuePair.Key]);
            }
        }

        [Test]
        public void TestGenerateRandomMap() {

            Dictionary<Vector2S, double> actual = FlyingIslands.GenerateRandomMap(0, 0, 1, -229);
            var serialized = JsonConvert.SerializeObject(actual);
            Assert.AreEqual("{\"[Vector: 0, 0]\":0.8302870738261845,\"[Vector: 0, 1]\":0.8859546047096956,\"[Vector: 1, 0]\":0.7594661478942726,\"[Vector: 1, 1]\":0.7946885459168698}", serialized);
        }

        [Test]
        public void TestDiamondRandomMap() {
            var expected =
                "{\"[Vector: 0, 0]\":0.8302870738261845,\"[Vector: 0, 1]\":0.8859546047096956,\"[Vector: 1, 0]\":0.7594661478942726,\"[Vector: 1, 1]\":0.8175990930867556,\"[Vector: 2, 2]\":0.7946885459168698,\"[Vector: 2, 0]\":0.7594661478942726,\"[Vector: 0, 2]\":0.8859546047096956}";
            Dictionary<Vector2S, double> actual = FlyingIslands.GenerateRandomMap(0, 0, 1, -229);
            var doubled = FlyingIslands.DoubleHeightMap(actual, 1);
            var ugh = FlyingIslands.DiamondRandomMap(doubled, 2, 32, 0, 0, -229, 0);

            var serialized = JsonConvert.SerializeObject(ugh);
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void TestSquareRandomMap() {
            var expected =
                "{\"[Vector: 0, 0]\":0.8302870738261845,\"[Vector: 0, 1]\":0.85812083926794,\"[Vector: 1, 0]\":0.7948766108602285,\"[Vector: 1, 1]\":0.7946885459168698,\"[Vector: 2, 2]\":0.7946885459168698,\"[Vector: 2, 0]\":0.7594661478942726,\"[Vector: 0, 2]\":0.8859546047096956,\"[Vector: 2, 1]\":0.7770773469055712,\"[Vector: 1, 2]\":0.8403215753132827}";
            Dictionary<Vector2S, double> actual = FlyingIslands.GenerateRandomMap(0, 0, 1, -229);
            var doubled = FlyingIslands.DoubleHeightMap(actual, 1);
            var ugh = FlyingIslands.SquareRandomMap(doubled, 2, 32, 0, 0, -229, 0);

            var serialized = JsonConvert.SerializeObject(ugh);
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void TestDiamondHeightMap() {
            var expected =
                "{\"[Vector: 0, 0]\":0.8302870738261845,\"[Vector: 0, 1]\":0.85812083926794,\"[Vector: 1, 0]\":0.7948766108602285,\"[Vector: 1, 1]\":0.8252209042602772,\"[Vector: 2, 2]\":0.8175990930867556,\"[Vector: 2, 0]\":0.7948766108602285,\"[Vector: 0, 2]\":0.85812083926794,\"[Vector: 2, 1]\":0.7770773469055712,\"[Vector: 1, 2]\":0.8403215753132827,\"[Vector: 4, 4]\":0.7946885459168698,\"[Vector: 4, 2]\":0.7770773469055712,\"[Vector: 4, 0]\":0.7594661478942726,\"[Vector: 2, 4]\":0.8403215753132827,\"[Vector: 0, 4]\":0.8859546047096956,\"[Vector: 1, 3]\":0.8504990280944185,\"[Vector: 3, 1]\":0.787254799686707,\"[Vector: 3, 3]\":0.8074216403056198}";
            Dictionary<Vector2S, double> actual = FlyingIslands.GenerateRandomMap(0, 0, 1, -229);
            actual = FlyingIslands.DoubleHeightMap(actual, 1);
            actual = FlyingIslands.DiamondHeightMap(actual, 2);
            actual = FlyingIslands.SquareHeightMap(actual, 2);

            actual = FlyingIslands.DoubleHeightMap(actual, 2);
            actual = FlyingIslands.DiamondHeightMap(actual, 4);

            var serialized = JsonConvert.SerializeObject(actual);
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void TestDoubleHeightMap() {
            var givenHeightmap = new Dictionary<Vector2S, double> { // -- AFter doubling this..1
                {new Vector2S(0, 0), 0},
                {new Vector2S(0, 1), 0},
                {new Vector2S(1, 0), 0},
                {new Vector2S(1, 1), 0},
            };

            var actualOne = FlyingIslands.DoubleHeightMap(givenHeightmap, 1);
            var expectedHeightMapOne = new Dictionary<Vector2S, double> { // -- It will equal this.(1) AFter doubling this..2 
                {new Vector2S(0, 0), 0},
                {new Vector2S(0, 1), 0},
                {new Vector2S(1, 0), 0},
                {new Vector2S(1, 1), 0},
                {new Vector2S(2, 2), 0},
                {new Vector2S(2, 0), 0},
                {new Vector2S(0, 2), 0},
            };
            AssertDictionaryEqual(expectedHeightMapOne, actualOne);

            var actualTwo = FlyingIslands.DoubleHeightMap(expectedHeightMapOne, 2);
            var expectedHeightMapTwo = new Dictionary<Vector2S, double> { // -- It will equal this(2)
                {new Vector2S(0, 0), 0},
                {new Vector2S(0, 1), 0},
                {new Vector2S(1, 0), 0},
                {new Vector2S(1, 1), 0},
                {new Vector2S(2, 2), 0},
                {new Vector2S(2, 0), 0},
                {new Vector2S(0, 2), 0},

                {new Vector2S(0, 4), 0},
                {new Vector2S(4, 0), 0},
                {new Vector2S(4, 4), 0},
                {new Vector2S(0, 8), 0},
                {new Vector2S(8, 0), 0},
                {new Vector2S(8, 8), 0},
            };
            AssertDictionaryEqual(expectedHeightMapTwo, actualTwo);
        }

        [Test]
        public void TestDoubleHeightMap_biggerSize() {
            var givenHeightmap = new Dictionary<Vector2S, double> {
                {new Vector2S(0, 0), 0},
                {new Vector2S(0, 1), 0},
                {new Vector2S(1, 0), 0},
                {new Vector2S(1, 1), 0},
                {new Vector2S(2, 2), 0},
                {new Vector2S(2, 1), 0},
                {new Vector2S(1, 2), 0},
                {new Vector2S(0, 2), 0},
                {new Vector2S(2, 0), 0},
            };

            var ogExpanded = doubleHeightMapOg(givenHeightmap, 4);
            var flyingExpaneded = FlyingIslands.DoubleHeightMap(givenHeightmap, 4);

            foreach (KeyValuePair<Vector2S, double> d in ogExpanded) {
                Assert.AreEqual(ogExpanded[d.Key], flyingExpaneded[d.Key]);
            }

            Assert.AreEqual(ogExpanded.Count, flyingExpaneded.Count);
        }
    }
}
