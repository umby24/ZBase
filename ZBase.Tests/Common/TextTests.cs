using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZBase.Common;


namespace ZBase.Tests.Common {
    [TestClass]
    public class TextTests {
        [TestMethod]
        public void RemoveColorsTest()
        {
            var givenInput = "&4I am a &csystem test &Fmessage!";
            var expected = "I am a system test message!";

            var actual = Text.RemoveColors(givenInput);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveColorsNegativeTest()
        {
            var givenInput = "&qI am a &zsystem test &rmessage!";

            var actual = Text.RemoveColors(givenInput);

            Assert.AreEqual(givenInput, actual);
        }

        [TestMethod]
        public void TestSplitLines()
        {
            var tests = Properties.Resources.textsplits;
            var testExpected = Properties.Resources.textsplit_expected;

            var actual = "";

            var splitTests = tests.Split('\n');

            foreach(var item in splitTests)
            {
                var result = Text.SplitLines(Text.RemoveColors(item));
                foreach (var rItem in result)
                {
                    actual += rItem + "\n";
                }
            }

            Assert.AreEqual(testExpected, actual);
        }
    }
}
