using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase {
    public class Watchdog : TaskItem {
        public static Dictionary<string, WatchdogModule> Modules;
        private static readonly object Mutex = new object();
        private static double _timer;
        #region HTML Header..
        private const string HtmlHeaders = @"<html>
    <head>
        <title>Hypercube Watchdog</title>
        <style type=""text/css"">
            body {
                font-family: ""Microsoft PhagsPa"";
                color:#2f2f2f;
                background-color:#F7F7F7;
            }
            h1.header {
                background-color:darkblue;
                text-shadow:2px 1px 0px rgba(0,0,0,.2); 
                font-size:25px;
                font-weight:bold;
                text-decoration:none;
                text-align:center;
                color:white;
                margin:0;
                height:42px; 
                width:auto;
                border-bottom: 1px black solid;
                height: 42px;
                margin: -8px;
                line-height: 42px;
            }
            table {
                border: 1px solid #A0A0A0;
                table-layout: auto;
                empty-cells: hide;
                border-collapse: collapse;
            }
            tr {
                border: 1px solid #A0A0A0;
                background-color: #D0D0D0;
                color: #212121;
                opacity:1.0;
            }
            td {
                border-right: 1px solid #A0A0A0;
            }
            th {
                border-right: 1px solid #A0A0A0;
            }
        </style>
    </head>
    
    <body>
        <h1 class=""header"">Hypercube Watchdog (Server stats)</h1>";
        #endregion
        public Watchdog() {
            LastRun = new DateTime();
            Interval = TimeSpan.FromSeconds(5);
        }

        public static void AddModule(WatchdogModule module) {
            if (Modules.ContainsKey(module.Name))
                return;

            Modules.Add(module.Name, module);
        }

        public static void Watch(string module, string message, bool endRecord) {
            lock (Mutex) {
                WatchdogModule mod;

                if (!Modules.TryGetValue(module, out mod)) {
                    return;
                }

                mod.Timeout = (int) (Milliseconds() - TimeToMilliseconds(mod.WatchTime)); // -- Time between last call and this call.

                if (mod.BiggestTimeout < mod.Timeout) { // -- Track the biggest time for a call..
                    mod.BiggestTimeout = mod.Timeout;
                    mod.BiggestMessage = mod.LastMessage;
                }

                mod.LastMessage = message; // -- Set the watch message.

                if (endRecord) { // -- Measure the calls per second.
                    mod.CallsPerSecond += 1;
                }

                mod.WatchTime = DateTime.UtcNow;
            }
        }

        public override void Setup() {
            Modules = new Dictionary<string, WatchdogModule> {
                {
                    "TaskScheduler", new WatchdogModule {
                        Name = "TaskScheduler",
                        TimeoutMax = 50
                    }
                },
                {
                    "Network IO", new WatchdogModule {
                        Name = "Network IO",
                        TimeoutMax = 50
                    }
                }
            };


            Logger.Log(LogType.Info, "=== Watchdog on duty ===");
        }

        public static double Milliseconds() {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static double TimeToMilliseconds(DateTime time) {
            return (time- new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private static void GenerateNotPretty() {
            string watchdogText = "Watchdog-notpretty" + Environment.NewLine;

            double time = Milliseconds() - _timer;
            _timer = Milliseconds();

            foreach (WatchdogModule module in Modules.Values) {
                module.Timeout = (int)(Milliseconds() - TimeToMilliseconds(module.WatchTime));

                if (module.BiggestTimeout < module.Timeout) {
                    module.BiggestTimeout = module.Timeout;
                }

                if (module.BiggestTimeout > module.TimeoutMax) {
                    watchdogText += "LAGGING:" + Environment.NewLine;
                }
                else {
                    watchdogText += "Well:" + Environment.NewLine;
                }

                watchdogText +=
                    $"{module.Name} -- {module.BiggestTimeout}ms ({module.TimeoutMax}ms) -- {module.BiggestMessage} -- {module.LastMessage} -- {module.CallsPerSecond * 1000 / time}/s" + Environment.NewLine + Environment.NewLine;

                module.CallsPerSecond = 0;
                module.BiggestTimeout = 0;
            }


            File.WriteAllText("Watchdog.txt", watchdogText);
        }

        private static void GenerateHtml() {
            var page = HtmlHeaders;
            page += "\t\t<p>Generated at " + DateTime.UtcNow.ToLongTimeString() + " (All times are UTC +0)</p>\n";

            // -- WD Modules...
            page += "\t\t<h3>Watch Modules:</h3>\n";
            page += "\t\t<table>\n";
            page += "<th>Module</th>\n\t\t\t<th>Biggest Timeout(Max)</th>\n\t\t\t" +
                    "\n\t\t\t<th>Biggest Message</th>\n\t\t\t<th>Last Message</th>\n\t\t\t<th>Calls a second</th>\n";
            double time = Milliseconds() - _timer;
            _timer = Milliseconds();

            foreach (WatchdogModule module in Modules.Values) {
                module.Timeout = (int)(Milliseconds() - TimeToMilliseconds(module.WatchTime));

                if (module.BiggestTimeout < module.Timeout) {
                    module.BiggestTimeout = module.Timeout;
                }
                var wdText = "";
                if (module.BiggestTimeout > module.TimeoutMax) {
                    wdText += "LAGGING:";
                }
                else {
                    wdText += "Well:";
                }

//                watchdogText +=
//                    $"{module.Name} -- {module.BiggestTimeout}ms ({module.TimeoutMax}ms) -- {module.BiggestMessage} -- {module.LastMessage} -- {module.CallsPerSecond * 1000 / time}/s" + Environment.NewLine + Environment.NewLine;
                page += "\t\t\t<tr>\n";
                page += "\t\t\t\t<td>" + module.Name + "(" + wdText + ")</td>\n";
                page += "\t\t\t\t<td>" + module.BiggestTimeout + "ms ("+module.TimeoutMax + "ms)</td>\n";
                page += "\t\t\t\t<td>" + module.BiggestMessage + "</td>\n";
                page += "\t\t\t\t<td>" + module.LastMessage + "</td>\n";
                page += "\t\t\t\t<td>" + (module.CallsPerSecond * 1000) / time + "</td>\n";
                module.CallsPerSecond = 0;
                module.BiggestTimeout = 0;
            }
            page += "\t\t</table>\n";
            // -- Scheduled Tasks
            page += "\t\t<h3>Tasks:</h3>\n";
            page += "\t\t<table>\n";
            page += "<th>Task Name</th>\n\t\t\t<th>Run Interval</th>\n\t\t\t<th>Last Run</th>\n";


            foreach (var task in TaskScheduler.Tasks) {
                page += "\t\t\t<tr>\n";
                page += "\t\t\t\t<td>" + task.Key + "</td>\n";
                page += "\t\t\t\t<td>" + task.Value.Interval + "</td>\n";
                page += "\t\t\t\t<td>" + task.Value.LastRun.ToLongTimeString() + "</td>\n";
                page += "\t\t\t</tr>\n";
            }

            page += "\t\t</table>\n";

            // -- Maps
            page += "\t\t<h3>Maps:</h3>\n";
            page += "\t\t<table>\n";
            page += "<th>Map Name</th>\n\t\t\t<th>Filename</th>\n\t\t\t<th>Size</th>\n";
            page += "\t\t\t<th>Loaded</th>\n\t\t\t<th>Blockchanging</th>\n\t\t\t<th>Physics</th>";
            page +=
                "\n\t\t\t<th>History</th>\n\t\t\t<th>Physics Queue</th>\n\t\t\t<th>Send Queue</th>\n\t\t\t<th>Clients</th>\n";

            foreach (var map in HcMap.Maps.Values) {
                var mapSize = map.GetSize();
                page += "\t\t\t<tr>\n";
                page += "\t\t\t\t<td>" + map.MapProvider.MapName + "</td>\n";
                page += "\t\t\t\t<td>" + map.Filename + "</td>\n";
                page += "\t\t\t\t<td>" + mapSize.X + "x" + mapSize.Y + "x" + mapSize.Z + "</td>\n";
                page += "\t\t\t\t<td>" + map.Loaded + "</td>\n";
  //              page += "\t\t\t\t<td>" + map.HCSettings.Building + "</td>\n";
  //              page += "\t\t\t\t<td>" + map.HCSettings.Physics + "</td>\n";
  //              page += "\t\t\t\t<td>" + map.HCSettings.History + "</td>\n";
  //              page += "\t\t\t\t<td>" + map.PhysicsQueue.Count + "</td>\n";
  //              page += "\t\t\t\t<td>" + map.BlockchangeQueue.Count + "</td>\n";
                page += "\t\t\t\t<td>" + Server.RoClients.Count(a => a.ClientPlayer != null && a.ClientPlayer.Entity?.CurrentMap == map) + "</td>\n";
                page += "\t\t\t</tr>\n";
            }

            page += "\t\t</table>\n";

            // -- Network info
            page += "\t\t<h3>Network:</h3>\n";
            page += "\t\tPort: " + Configuration.Settings.Network.ListenPort + "<br>\n";
            page += "\t\tPublic: " + Configuration.Settings.Network.Public + "<br>\n";
            page += "\t\tVerify Names?: " + Configuration.Settings.Network.VerifyNames + "<br>\n";
            page += "\t\tServer URL: " + Heartbeat.ServerUrl + "<br>\n";
            page += "\t\tClients (Logged): " + Server.RoClients.Length + "<br>\n\n";

            page += "\t\t<h4>Clients:</h4>\n";
            page += "\t\t<table>\n";
            page += "\t\t\t<th>ID</th>\n\t\t\t<th>Login Name</th>\n\t\t\t<th>IP</th>\n\t\t\t";
            page += "\n\t\t\t<th>Supports CPE</th>\n\t\t\t<th>Appname</th>\n\t\t\t<th>Extensions</th>\n\t\t\t<th>Map</th>";
            page += "\n\t\t\t<th>Entity ID</th>\n\t\t\t<th>Send Queue</th>\n";

            foreach (var client in Server.RoClients) {
                if (client?.ClientPlayer?.Entity == null)
                    continue;

                page += "\t\t\t<tr>\n";
             //   page += "\t\t\t\t<td>" + client.CS.Id + "</td>\n";
                page += "\t\t\t\t<td>" + client.ClientPlayer.Entity.Name + "</td>\n";
                page += "\t\t\t\t<td>" + client.Ip + "</td>\n";
                //page += "\t\t\t\t<td>" + client.IsCpe + "</td>\n";
                //page += "\t\t\t\t<td>" + client.App + "</td>\n";
                //page += "\t\t\t\t<td>" + client.ExtensionsCount + "</td>\n";
                page += "\t\t\t\t<td>" + client.ClientPlayer.Entity.CurrentMap.MapProvider.MapName + "</td>\n";
                page += "\t\t\t\t<td>" + client.ClientPlayer.Entity.ClientId + "</td>\n";// + "(" + client.CS.MyEntity.ClientId + ")" + "</td>\n";
                page += "\t\t\t\t<td>" + client.SendBuffer.Length + "</td>\n";
                page += "\t\t\t</tr>\n";
            }

            page += "\t\t</table>\n";
            page += "\t</body>\n";
            page += "</html>";

            if (!Directory.Exists("HTML"))
                Directory.CreateDirectory("HTML");

            File.WriteAllText("HTML/Watchdog.html", page);
        }

        public override void Main() {
            lock (Mutex) {
                GenerateHtml();
            //    GenerateNotPretty();
            }

        }

        public override void Teardown() {
        }
    }
}
