using System;
using ZBase.Network;
using ZBase.World;

namespace ZBaseConsole {
    class Program {
        static void Main(string[] args) {
            ZBase.Main.Start();

            string input;
            while (true) {
                input = Console.ReadLine();
                
                if (input.ToLower() == "q")
                    break;

                HandleInput(input);
            }

            Console.ReadKey();
            ZBase.Main.Stop();
            Console.ReadKey();
        }

        static void HandleInput(string input) {
            if (!input.StartsWith("/")) {
                Chat.SendGlobalChat("&c[CONSOLE]&f: " + input, 0);
            }

            if (input.StartsWith("/opme")) {
                var model = Player.Database.GetPlayerModel("umby24");
                model.Rank = (short) short.MaxValue;
                Player.Database.UpdatePlayer(model);
            }
        }
    }
}