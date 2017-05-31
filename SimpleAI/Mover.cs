using System;
using System.Linq;

namespace SimpleAI
{
    internal class Mover
    {
        private const int Enter = 13;
        private const int Plus = 187;
        private const int Minus = 189;

        public static void Run()
        {
            var service = new DeepThink(220, 221, 4, 4);
            var x = 15;
            var y = 7;
            DrawScreen(x, y, "");

            var lastOutputs = new double[] { };

            var reinforcing = false;
            while (true)
            {
                var keyDown = (int)Console.ReadKey(true).Key;
                switch (keyDown)
                {
                    case Plus:
                        service.Reward();
                        DrawScreen(x, y, "Rewarded");
                        break;
                    case Minus:
                        service.Punish();
                        DrawScreen(x, y, "Punished");
                        break;
                    case Enter:
                        reinforcing = true;
                        DrawScreen(x, y, "Training... Press a key to reinforce it.");
                        break;
                    default:
                        service.Reset();
                        var inputs = new double[220];
                        for (var n = 0; n < 220; n++) { inputs[n] = 0D; }
                        inputs[keyDown] = 1D;

                        if (reinforcing)
                        {
                            service.Train(inputs, lastOutputs);
                            reinforcing = false;
                            DrawScreen(x, y, "Trained");
                        }
                        else
                        {
                            lastOutputs = service.Run(inputs, false).Select(v => Math.Round(v)).ToArray();

                            if (lastOutputs[0] == 1) x -= 1;
                            if (lastOutputs[1] == 1) y -= 1;
                            if (lastOutputs[2] == 1) x += 1;
                            if (lastOutputs[3] == 1) y += 1;

                            if (x < 1) x = 30;
                            if (x > 30) x = 1;
                            if (y < 1) y = 15;
                            if (y > 15) y = 1;

                            Console.Clear();
                            DrawScreen(x, y, "");
                        }
                        break;
                }
            }
        }

        protected static void DrawScreen(int x, int y, string message)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Press any key except Enter/-/+ to Train the thing to move. Press + to reward, - to Punish");
            Console.WriteLine("Press Enter to punish if it moved the wrong way!");

            Console.SetCursorPosition(0, 3);
            Console.WriteLine("  ╔╗                              ╔╗");
            Console.WriteLine("  ╚╬══════════════════════════════╬╝");
            for (var n = 1; n <= 15; n++)
            {
                Console.WriteLine("   ║                              ║ ");

            }
            Console.WriteLine("  ╔╬══════════════════════════════╬╗");
            Console.WriteLine("  ╚╝                              ╚╝");

            Console.SetCursorPosition(x + 3, y + 4);
            Console.Write("O");
            Console.SetCursorPosition(4, 22);
            Console.Write(message);
        }
    }
}
