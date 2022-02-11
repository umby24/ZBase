using System;
using System.Text.RegularExpressions;
using ZBase.Commands;
using ZBase.Common;
using ZBase.World;

namespace ZBase.Network {
    public static class Chat {
        private const string MutedMessage = "You are muted!";

        public static event StringEventArgs GlobalChatSent;

        public static void SendGlobalChat(string message, sbyte messageType, bool log = false) {
            message = Text.CleanseStringCP437(message);
            message = EmoteReplace(message);
            GlobalChatSent?.Invoke(message);

            if (log)
                Logger.Log(LogType.Chat, message);
        }

        public static void SendMapChat(string message, sbyte messageType, HcMap map, bool log = false) {
            message = Text.CleanseString(message);
            message = EmoteReplace(message);
            map.InvokeMapChat(message);

            if (log)
                Logger.Log(LogType.Chat, "[To Map " + map.MapProvider.MapName + "]: " + message);
        }

        public static void SendClientChat(string message, sbyte messageType, INetworkClient recipient, bool log = false) {
            message = Text.CleanseString(message);
            message = EmoteReplace(message);
            // -- Event?
            if (log)
                Logger.Log(LogType.Chat, "[To: " + recipient.Name + "]: " + message);

            recipient.GetPlayerInstance().HandleChatReceived(message);
        }

        public static void SendClientChat(string message, sbyte messageType, Client recipient, bool log = false) {
            message = Text.CleanseString(message);
            message = EmoteReplace(message);
            // -- Event?
            if (log)
                Logger.Log(LogType.Chat, "[To: " + recipient.ClientPlayer.Name + "]: " + message);

            recipient.ClientPlayer.HandleChatReceived(message);
        }

        public static void HandleIncoming(INetworkClient c, string message, bool extend = false) {
            //if (c.ClientPlayer.MutedUntil >= DateTime.UtcNow) {
            //    SendClientChat(Constants.SystemColor + MutedMessage, 0, c);
            //    return;
            //}

            if (message.StartsWith(CommandHandler.CommandPrefix)) {
                //CommandHandler.HandleCommand(c, message);
                return;
            }

            //if (extend) {
            //    c.ClientPlayer.ChatBuffer += message;
            //    return;
            //}
            if (message.Contains("/trychangemap")) {
                HcMap newMap;

                if (!HcMap.Maps.TryGetValue("derp", out newMap)) {
                    SendClientChat($"§EMap 'derp' not found.", 0, c);
                    return;
                }


                c.ClientPlayer.ChangeMap(newMap);
            }
            SendGlobalChat(c.ClientPlayer.Entity.PrettyName + Constants.DefaultColor + ": " + message, 0, true);
            //c.ClientPlayer.ChatBuffer = "";
        }

        public static void HandleIncoming(Client c, string message, bool extend = false) {
            if (c.ClientPlayer.MutedUntil >= DateTime.UtcNow) {
                SendClientChat(Constants.SystemColor + MutedMessage, 0, c);
                return;
            }

            if (message.StartsWith(CommandHandler.CommandPrefix)) {
                CommandHandler.HandleCommand(c, message);
                return;
            }

            if (extend) {
                c.ClientPlayer.ChatBuffer += message;
                return;
            }

            SendGlobalChat(c.ClientPlayer.Entity.PrettyName + Constants.DefaultColor + ": " + c.ClientPlayer.ChatBuffer + message, 0, true);
            c.ClientPlayer.ChatBuffer = "";
        }

        public static bool FirstIsEmote(char first) {
            var regex = new Regex("[^\u0001-\u007F]+");
            return regex.IsMatch(first.ToString());
        }

        /// <summary>
        /// Replaces in-game text codes with emotes (unicode).
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string EmoteReplace(string message) {
            message = message.Replace("{:)}", "\u0001"); // ☺
            message = message.Replace("{smile}", "\u0001");

            message = message.Replace("{smile2}", "\u0002"); // ☻

            message = message.Replace("{heart}", "\u0003"); // ♥
            message = message.Replace("{hearts}", "\u0003");
            message = message.Replace("{<3}", "\u0003");

            message = message.Replace("{diamond}", "\u0004"); // ♦
            message = message.Replace("{diamonds}", "\u0004");
            message = message.Replace("{rhombus}", "\u0004");

            message = message.Replace("{club}", "\u0005"); // ♣
            message = message.Replace("{clubs}", "\u0005");
            message = message.Replace("{clover}", "\u0005");
            message = message.Replace("{shamrock}", "\u0005");

            message = message.Replace("{spade}", "\u0006"); // ♠
            message = message.Replace("{spades}", "\u0006");

            message = message.Replace("{*}", "\u0007"); // •
            message = message.Replace("{bullet}", "\u0007");
            message = message.Replace("{dot}", "\u0007");
            message = message.Replace("{point}", "\u0007");

            message = message.Replace("{hole}", "\u0008"); // ◘

            message = message.Replace("{circle}", "\u0009"); // ○
            message = message.Replace("{o}", "\u0009");

            message = message.Replace("{male}", "\u000B"); // ♂
            message = message.Replace("{mars}", "\u000B");

            message = message.Replace("{female}", "\u000C"); // ♀
            message = message.Replace("{venus}", "\u000C");

            message = message.Replace("{8}", "\u000D"); // ♪
            message = message.Replace("{note}", "\u000D");
            message = message.Replace("{quaver}", "\u000D");

            message = message.Replace("{notes}", "\u000E"); // ♫
            message = message.Replace("{music}", "\u000E");

            message = message.Replace("{sun}", "\u000F"); // ☼
            message = message.Replace("{celestia}", "\u000F");

            message = message.Replace("{>>}", "\u0010"); // ►
            message = message.Replace("{right2}", "\u0010");

            message = message.Replace("{<<}", "\u0011"); // ◄
            message = message.Replace("{left2}", "\u0011");

            message = message.Replace("{updown}", "\u0012"); // ↕
            message = message.Replace("{^v}", "\u0012");

            message = message.Replace("{!!}", "\u0013"); // ‼

            message = message.Replace("{p}", "\u0014"); // ¶
            message = message.Replace("{para}", "\u0014");
            message = message.Replace("{pilcrow}", "\u0014");
            message = message.Replace("{paragraph}", "\u0014");

            message = message.Replace("{s}", "\u0015"); // §
            message = message.Replace("{sect}", "\u0015");
            message = message.Replace("{section}", "\u0015");

            message = message.Replace("{-}", "\u0016"); // ▬
            message = message.Replace("{_}", "\u0016");
            message = message.Replace("{bar}", "\u0016");
            message = message.Replace("{half}", "\u0016");

            message = message.Replace("{updown2}", "\u0017"); // ↨
            message = message.Replace("{^v_}", "\u0017");

            message = message.Replace("{^}", "\u0018"); // ↑
            message = message.Replace("{up}", "\u0018");

            message = message.Replace("{v}", "\u0019"); // ↓
            message = message.Replace("{down}", "\u0019");

            message = message.Replace("{>}", "\u001A"); // →
            message = message.Replace("{->}", "\u001A");
            message = message.Replace("{right}", "\u001A");

            message = message.Replace("{<}", "\u001B"); // ←
            message = message.Replace("{<-}", "\u001B");
            message = message.Replace("{left}", "\u001B");

            message = message.Replace("{l}", "\u001C"); // ∟
            message = message.Replace("{angle}", "\u001C");
            message = message.Replace("{corner}", "\u001C");

            message = message.Replace("{<>}", "\u001D"); // ↔
            message = message.Replace("{<->}", "\u001D");
            message = message.Replace("{leftright}", "\u001D");

            message = message.Replace("{^^}", "\u001E"); // ▲
            message = message.Replace("{up2}", "\u001E");

            message = message.Replace("{vv}", "\u001F"); // ▼
            message = message.Replace("{down2}", "\u001F");

            message = message.Replace("{house}", "\u007F"); // ⌂

            message = message.Replace("{caret}", "^");
            message = message.Replace("{hat}", "^");

            message = message.Replace("{tilde}", "~");
            message = message.Replace("{wave}", "~");

            message = message.Replace("{grave}", "`");
            message = message.Replace("{\"}", "`");
            return message;
        }
    }
}
