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
        static int ENTERKEY = 13;

        private static double _Errorthresh = 0.01;

        static void Main(string[] args)
        {
            LogicGateAI();

            Console.WriteLine("Press any key to end the program...");
            Console.ReadKey();
        }

        static void MoverAI()
        {
            var Service = new DeepThink(1, 1, 4, 4);
            var x = 10;
            var y = 10;

            while (true)
            {
                DrawScreen(0, 0);
                var keyDown = (int) Console.ReadKey().Key;
                if (keyDown == ENTERKEY)
                {
                    Service.Punish();
                }
                else
                {
                    var outputs = Service.Run(new[] {keyDown / 220D}).Select(v => Math.Round(v)).ToArray();
                    if (outputs[0] != 0) x -= 1;
                    if (outputs[1] != 0) y -= 1;
                    if (outputs[2] != 0) x += 1;
                    if (outputs[3] != 0) y += 1;
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
        static void DrawScreen(int x, int y)
        {
            if (x < 1) x = 1;
            if (x > 30) x = 30;
            if (y < 1) y = 1;
            if (y > 15) y = 15;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Press any key except Enter to encourage the thing to move.");
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
            Console.SetCursorPosition(0, 21);
        }
    }
}
