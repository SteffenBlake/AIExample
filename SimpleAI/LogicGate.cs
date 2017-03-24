using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAI.Models;

namespace SimpleAI
{
    public class LogicGate
    {
        private const double _Errorthresh = 0.01;

        public static void Run()
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

    }
}
