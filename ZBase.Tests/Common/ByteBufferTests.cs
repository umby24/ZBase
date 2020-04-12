using NUnit.Framework;
using ZBase.Common;

namespace ZBase.Tests.Common {
    public class ByteBufferTests {
        private ByteBuffer _testBuffer;

        [SetUp]
        public void Setup() {
            _testBuffer = new ByteBuffer();    
        }
        
        [Test]
        public void AddBytesTest() {
            var givenData = new byte[] {0, 1, 2, 3};
            
            _testBuffer.AddBytes(givenData);
            var actualBufferLength = _testBuffer.Length;
            var expectedBufferLength = 4;
            
            Assert.AreEqual(expectedBufferLength, actualBufferLength);
        }

        [Test]
        public void AddBytesNullTest() {
            _testBuffer = new ByteBuffer();
            
            var givenData = new byte[] {0, 1, 2, 3};
            _testBuffer.AddBytes(givenData);
            _testBuffer.AddBytes(null);

            var actualLength = _testBuffer.Length;
            var expectedLength = 4;
            
            Assert.AreEqual(expectedLength, actualLength);
        }

        [Test]
        public void AddBytesSubsequentTest() {
            _testBuffer = new ByteBuffer();
            
            var givenData = new byte[] {0, 1, 2, 3};
            var givenOtherData = new byte[] {4, 5, 6, 7};
            
            _testBuffer.AddBytes(givenData);
            _testBuffer.AddBytes(givenOtherData);
            
            var actualBufferLength = _testBuffer.Length;
            var expectedBufferLength = 8;
            
            Assert.AreEqual(expectedBufferLength, actualBufferLength);
        }

        [Test]
        public void GetAllBytesTest() {
            _testBuffer = new ByteBuffer();
            
            var givenData = new byte[] {0, 1, 2, 3};
            var givenOtherData = new byte[] {4, 5, 6, 7};
            
            _testBuffer.AddBytes(givenData);
            _testBuffer.AddBytes(givenOtherData);

            var expectedData = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
            var actualData = _testBuffer.GetAllBytes();
            
            Assert.AreEqual(expectedData, actualData); // -- data should be equal
            Assert.AreEqual(_testBuffer.Length, 0); // -- Buffer should be cleared.
        }
    }
}