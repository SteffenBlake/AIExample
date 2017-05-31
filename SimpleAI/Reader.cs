using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleAI
{
    public class Reader
    {
        private static readonly int A = Convert.ToInt32('a') - 1;

        private const double ErrorThresh = 0.05D;

        public static void Run()
        {
            var dir = Directory.GetCurrentDirectory().Replace("bin\\Debug", "dictionary.txt");
            var reader = new StreamReader(dir);

            var wordList = new List<string>();
            

            while (reader.Peek() != 0)
            {
                var nextLine = reader.ReadLine();
                if (nextLine == null) break;
                wordList.Add(nextLine.Trim().ToLower());
            }

            var wordArray = wordList.Select(s => s.Select(ToSimple).ToArray()).ToArray();

            var maxLength = wordList.Select(a => a.Length).Max();
            
            var service = new DeepThink(1, maxLength, maxLength+1, 1);

            //  B      C      D      F      G      H      J       K       L      M        N       P      Q        R       S      T       V       W        X        Y     Z 
            var consanents = new List<double>() { 2 / 26D, 3 / 26D, 4 / 26D, 6 / 26D, 7 / 26D, 8 / 26D, 10 / 26D, 11 / 26D, 12 / 26D, 13 / 26D, 14 / 26D, 16 / 26D, 17 / 26D, 18 / 26D, 19 / 26D, 20 / 26D, 21 / 26D, 22 / 26D, 23 / 26D, 24 / 26D, 1D }.ToArray();
            var rnd = new Random();

            for (int n=0, max = wordArray.Length; n < max; n++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Iterations: {0}/{1}", n, max);
                var word = wordArray[n];
                service.Train(word, new[] { 1D });
                var error1 = service.LastError;

                var length = word.Length;
                var badArray = new double[length];
                for (var y = 0; y < length; y++)
                {
                    badArray[y] = consanents[rnd.Next(0, 21)];
                }
                var output = service.Run(badArray);
                service.Train(badArray, new[] { 0D });
                var error2 = service.LastError;
                var totalError = Math.Sqrt(error1 * error1 + error2 * error2);
                Console.WriteLine("Last Error: {0:P}", totalError);
            }

            Console.WriteLine("Ready! Give me words to try and guess! Type 'Exit' to end");
            while (true)
            {
                var input = Console.ReadLine().Trim().ToLower();
                if (input == "exit") break;

                if (input.All(char.IsLetter))
                {
                    var charArray = input.Select(ToSimple).ToArray();
                    var output = service.Run(charArray).First();
                    Console.WriteLine("Confidence: {0:P}", output);
                }
                else
                {
                    Console.WriteLine("Now I know that's definitely not a word! Try again!");
                }
            }
        }

        private static double ToSimple(char c)
        {
            return Convert.ToInt32(c) - A / 26D;
        }
    }
}
