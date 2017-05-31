using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleAI
{
    public class LogicGate
    {
        private const double Errorthresh = 0.01;

        public static void Run()
        {
            var service = new DeepThink(1, 2, 4, 1);

            var trainingData = new Dictionary<double[], double[]>
            {
                { new[] {0D, 0D}, new[] {0D}},
                { new[] {0D, 1D}, new[] {1D}},
                { new[] {1D, 0D}, new[] {0D}},
                { new[] {1D, 1D}, new[] {1D}}
            };


            var currentError = 1D;
            for (var n = 0; currentError > Errorthresh; n++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Training - Sets: {0} - Error threshold: {1:P}", n, Errorthresh);
                var errorList = new List<double>();
                foreach (var trainer in trainingData)
                {
                    service.Train(trainer.Key, trainer.Value);
                    errorList.Add(service.LastError);
                }
                currentError = Math.Sqrt(errorList.Sum(e => e * e));
            }

            Console.WriteLine("Error: {0:P}", currentError);

            foreach (var trainer in trainingData)
            {
                Console.WriteLine("Inputs: {0}", string.Join("-", trainer.Key));
                Console.WriteLine("Expected Out: {0}", string.Join("-", trainer.Value));

                var outputs = service.Run(trainer.Key);
                Console.WriteLine("Actual Out: {0}\n", string.Join("-", outputs));
            }
        }

    }
}
