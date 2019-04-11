using System;
using ZBase.Network;

namespace Cli {
    class Program {
        static void Main(string[] args) {
            ZBase.Main.Start();
            string input;
            while (true) {
                input = Console.ReadLine();

                if (input == "q")
                    break;

                HandleInput(input);
            }

            Console.ReadKey();
            ZBase.Main.Stop();
            Console.WriteLine("Donezo");
            Console.ReadKey();
        }

        static void HandleInput(string input) {
            if (!input.StartsWith("/")) {
                Chat.SendGlobalChat("&c[CONSOLE]:&f " + input, 0);
            }
        }
    }
}
