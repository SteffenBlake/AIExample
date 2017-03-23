using SimpleAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI
{
    class Program
    {
        static int ENTER = 13;
        static int PLUS = 187;
        static int MINUS = 189;
        static double Min_Thresh = 0.5D;

        private static double _Errorthresh = 0.01;

        static void Main(string[] args)
        {
            MoverAI();

            Console.WriteLine("Press any key to end the program...");
            Console.ReadKey();
        }

        static void MoverAI()
        {
            var Service = new DeepThink(220, 221, 4, 4);
            var x = 15;
            var y = 7;
            DrawScreen(x, y, "");

            var lastOutputs = new double[] { };

            bool reinforcing = false;
            while (true)
            {
                var keyDown = (int)Console.ReadKey(true).Key;
                if (keyDown == PLUS)
                {
                    Service.Reward();
                    DrawScreen(x, y, "Rewarded");
                }
                else if (keyDown == MINUS)
                {
                    Service.Punish();
                    DrawScreen(x, y, "Punished");
                }
                else if (keyDown == ENTER)
                {
                    reinforcing = true;
                    DrawScreen(x, y, "Training... Press a key to reinforce it.");
                }
                else
                {
                    Service.Reset();
                    var inputs = new double[220];
                    for (int n = 0; n < 220; n++) { inputs[n] = 0D; }
                    inputs[keyDown] = 1D;

                    if (reinforcing)
                    {
                        Service.Train(inputs, lastOutputs);
                        reinforcing = false;
                        DrawScreen(x, y, "Trained");
                    }
                    else
                    {
                        lastOutputs = Service.Run(inputs,false).Select(v => Math.Round(v)).ToArray();

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
                }
            }
        }

        static void LogicGateAI()
        {
            var service = new DeepThink(1, 2, 4, 1);

            var TrainingData = new Dictionary<double[], double[]>();

            TrainingData.Add(new[] { 0D, 0D }, new[] { 0D });
            TrainingData.Add(new[] { 0D, 1D }, new[] { 1D });
            TrainingData.Add(new[] { 1D, 0D }, new[] { 0D });
            TrainingData.Add(new[] { 1D, 1D }, new[] { 1D });

            var currentError = 1D;
            for (int n = 0; currentError > _Errorthresh; n++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Training - Sets: {0} - Error threshold: {1:P}", n, _Errorthresh);
                var errorList = new List<double>();
                foreach (KeyValuePair<double[], double[]> trainer in TrainingData)
                {
                    service.Train(trainer.Key, trainer.Value);
                    errorList.Add(service.LastError);
                }
                currentError = Math.Sqrt(errorList.Sum(e => e * e));
            }

            Console.WriteLine("Error: {0:P}", currentError);

            foreach (KeyValuePair<double[], double[]> Trainer in TrainingData)
            {
                Console.WriteLine("Inputs: {0}", string.Join("-", Trainer.Key));
                Console.WriteLine("Expected Out: {0}", string.Join("-", Trainer.Value));

                var outputs = service.Run(Trainer.Key);
                Console.WriteLine("Actual Out: {0}\n", string.Join("-", outputs));
            }
        }

        //Draws the screen. X must be 1-30, y 1-15
        static void DrawScreen(int x, int y, string message)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Press any key except Enter/-/+ to Train the thing to move. Press + to reward, - to Punish");
            Console.WriteLine("Press Enter to punish if it moved the wrong way!");

            Console.SetCursorPosition(0, 3);
            Console.WriteLine("  ╔╗                              ╔╗");
            Console.WriteLine("  ╚╬══════════════════════════════╬╝");
            for (int n = 1; n <= 15; n++)
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
