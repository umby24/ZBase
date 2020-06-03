using System;
using NUnit.Framework;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.Tests.Network {
    public class HeartbeatTests {
        [SetUp]
        public void Setup() {
            Heartbeat._salt = "testsalt";
            Configuration.Load();
            Configuration.Settings.Network.VerifyNames = true;
        }
        
        [Test]
        public void VerifyPositive() {
            string givenClientIp = "200.200.200.200";
            string givenClientName = "testclient";
            string givenMppass = "7d6b294c910460f223cd10e527b3e82c"; // -- md5(testsalt + testclient)

            bool result = Heartbeat.Verify(givenClientIp, givenClientName, givenMppass);
            Assert.True(result);
        }
        
        [Test]
        public void VerifyNegative() {
            string givenClientIp = "200.200.200.200";
            string givenClientName = "testclient";
            string givenMppass = "1a40c94c87e1b1500314307dda413ce4"; // -- md5(badsalt + testclient)

            bool result = Heartbeat.Verify(givenClientIp, givenClientName, givenMppass);
            Assert.False(result);
        }
        
        [Test]
        public void VerifyLocalNetwork() {
            string givenClientIp = "127.0.0.1";
            string givenClientName = "testclient";
            string givenMppass = "1a40c94c87e1b1500314307dda413ce4"; // -- md5(badsalt + testclient)

            bool result = Heartbeat.Verify(givenClientIp, givenClientName, givenMppass);
            Assert.True(result);
        }

        [Test]
        public void CallClassicube() {
            string givenPort = "11111";
            string givenUsers = "0";
            string givenMax = "255";
            string givenServerName = "UnitTest";
            string givenPublic = "false";
            string givenSoftware = "ZBase";
            string givenSalt = "testsalt";

            string result = Heartbeat.CallClassicube(givenPort, givenUsers, givenMax, givenServerName, givenPublic, givenSoftware,
                givenSalt);
            
            Console.WriteLine(result);
            Assert.True(result.Contains("http"));
        }
    }
}