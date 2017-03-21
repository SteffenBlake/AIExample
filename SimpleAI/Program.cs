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
        static readonly int TrainingSets = 1000;
        static void Main(string[] args)
        {
            var TrainingData = new Dictionary<double[], double[]>();

            TrainingData.Add(new[] { 0D, 0D }, new[] { 0D });
            TrainingData.Add(new[] { 0D, 1D }, new[] { 1D });
            TrainingData.Add(new[] { 1D, 0D }, new[] { 1D });
            TrainingData.Add(new[] { 1D, 1D }, new[] { 0D });

            var Service = new DeepThink(1, 4, 4, 1);

            for (int n=0; n < TrainingSets; n++)
            {
                Console.WriteLine("Commencing Training! {0}/{1}", n, TrainingSets);
                foreach(KeyValuePair<double[], double[]> Trainer in TrainingData)
                {
                    Service.Train(Trainer.Key, Trainer.Value);
                }
                Console.Clear();
            }
            
            foreach (var key in TrainingData.Keys)
            {
                Console.WriteLine("Input: {0}", string.Join("-", key));
                var output = Service.Run(key);
                Console.WriteLine(string.Join("-", output));
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
