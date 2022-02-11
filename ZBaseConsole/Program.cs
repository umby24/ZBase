using System;
using System.Threading;
using ZBase.Network;
using ZBase.World;

namespace ZBaseConsole {
    class Program {
        static void Main(string[] args) {
            ZBase.Main.Start();
            
            string input;
            while (true) {
                input = Console.ReadLine();

                if (input == null) {
                    NoInputMode();
                    break;
                }
                
                if (input.ToLower() == "q")
                    break;

                HandleInput(input);
            }

            Console.ReadKey();
            ZBase.Main.Stop();
            Console.ReadKey();
        }
        static void NoInputMode() {
            bool running = true;
            
            Console.CancelKeyPress += delegate {
                running = false;
            };

            while (running) {
                Thread.Sleep(1);
            }
        }
        
        static void HandleInput(string input) {
            if (!input.StartsWith("/")) {
                Chat.SendGlobalChat("&c[CONSOLE]&f: " + input, 0);
            }

            if (input.StartsWith("/opme")) {
                var model = ClassicubePlayer.Database.GetPlayerModel("umby24");
                model.Rank = (short) short.MaxValue;
                ClassicubePlayer.Database.UpdatePlayer(model);
            }
        }
    }
}