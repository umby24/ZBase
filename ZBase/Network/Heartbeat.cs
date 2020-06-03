using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ZBase.Common;

namespace ZBase.Network {
    public class Heartbeat : TaskItem {
        private const string ClassicubeNetAddress = "classicube.net";
        public static string _salt;
        internal static string ServerUrl;
        private static byte _failCount;

        public override void Setup() {
            LastRun = new DateTime();
            Interval = new TimeSpan(0,0,45);
            CreateSalt();
        }

        public override void Main() {
            Beat();
        }

        public override void Teardown() {
            // -- Nothing
        }

        /// <summary>
        /// Determines if a given client's name is verified with classicube.net
        /// </summary>
        /// <param name="c"></param>
        /// <param name="name"></param>
        /// <param name="mppass"></param>
        /// <returns>true if verified, false otherwise.</returns>
        public static bool Verify(string clientIp, string name, string mppass) {
            if (clientIp == Constants.LocalhostNetwork || clientIp.Substring(0, 7) == Constants.LocalNetworkPrefix ||
                Configuration.Settings.Network.VerifyNames == false)
                return true;

            MD5 myMd5 = MD5.Create();
            string correct = BitConverter.ToString(myMd5.ComputeHash(Encoding.ASCII.GetBytes(_salt + name))).Replace("-", "");

            if (String.Equals(correct.Trim(), mppass.Trim(), StringComparison.CurrentCultureIgnoreCase))
                return true;

            Logger.Log(LogType.Warning, correct.Trim() + " != " + mppass.Trim());
            return false;
        }
        
        /// <summary>
        /// Returns the IPv4 Address of a site. (For classicube serverlist when on an IPv6 Supported host)
        /// </summary>
        /// <param name="site">The site to return the ipv4 address for</param>
        /// <returns>IPv4 Address of the given site.</returns>
        private static string GetIPv4Address(string site) {
            IPAddress[] addresses = Dns.GetHostAddresses(site);
            IPAddress v4 = addresses.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return v4.ToString();
        }

        public static string CallClassicube(string port, string users, string maxUsers, string serverName, string isPublic, string software, string salt) {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append($"http://www.{ClassicubeNetAddress}/heartbeat.jsp");
            urlBuilder.Append($"?port={port}");
            urlBuilder.Append($"&users={users}");
            urlBuilder.Append($"&max={maxUsers}");
            urlBuilder.Append($"&name={serverName}");
            urlBuilder.Append($"&public={isPublic}");
            urlBuilder.Append($"&software={software}");
            urlBuilder.Append($"&salt={salt}");

            var request = new WebClient {
                Proxy = new WebProxy($"http://{GetIPv4Address(ClassicubeNetAddress)}:80/")
            };
            
            return request.DownloadString(urlBuilder.ToString());
        }
        
        /// <summary>
        /// Performs a heartbeat to classicube.net
        /// </summary>
        private void Beat() {
            if (_failCount >= 3 && Interval.TotalSeconds == 45) {
                Logger.Log(LogType.Warning, "Failed to heartbeat 3+ times, increasing heartbeat delay..");
                Interval = new TimeSpan(0, 3, 0);
                _failCount = 0;
            } else if (_failCount >= 3) {
                _failCount = 0;
            }

            var port = Configuration.Settings.Network.ListenPort.ToString();
            var online = Server.OnlinePlayers.ToString();
            var max = Configuration.Settings.Network.MaxPlayers.ToString();
            var isPublic = Configuration.Settings.Network.Public.ToString();
            string name = HttpUtility.UrlEncode(Configuration.Settings.General.Name);
            string salt = HttpUtility.UrlEncode(_salt);
            string software = "ZBase";

            try {
                string response = CallClassicube(port, online, max, name, isPublic, software, salt);
                
                if (response.Contains("http")) {
                    Logger.Log(LogType.Info, "Heartbeat sent.");
                    Interval = new TimeSpan(0, 0, 45);
                }
                else {
                    Logger.Log(LogType.Warning, "Failed to send heartbeat: Unexpected response");
                    Logger.Log(LogType.Debug, $"Response: {response}");
                    _failCount += 1;
                }

                ServerUrl = response;
            } catch (Exception e) {
                Logger.Log(LogType.Warning, $"Failed to send heartbeat: {e.Message}");
                Logger.Log(LogType.Debug, $"Stack: {e.StackTrace}");
                _failCount += 1;
            }
        }

        private void CreateSalt() {
            _salt = "";
            var random = new Random();

            for (var i = 1; i < 33; i++)
                _salt += (char)(65 + random.Next(25));
        }
    }
}
