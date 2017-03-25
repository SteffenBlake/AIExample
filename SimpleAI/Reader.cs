using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAI.Models;

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

            var wordArray = wordList.Select(s => s.Select(c => (Convert.ToInt32(c) - A)/26D).ToArray()).ToArray();

            var maxLength = wordList.Select(a => a.Length).Max();
            
            var service = new DeepThink(1, maxLength, maxLength+1, 1);

            var expectedOut = new [] {1D};

            for (int n=0, max = wordArray.Length; n < max; n++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Iterations: {0}/{1}", n, max);
                service.Train(wordArray[n], expectedOut);
            }

            var error = 1D;
                                               //  B      C      D      F      G      H      J       K       L      M        N       P      Q        R       S      T       V       W        X        Y     Z 
            var consanents = new List<double>() {2/26D, 3/26D, 4/26D, 6/26D, 7/26D, 8/26D, 10/26D, 11/26D, 12/26D, 13/26D, 14/26D, 16/26D, 17/26D, 18/26D, 19/26D, 20/26D, 21/26D, 22/26D, 23/26D, 24/26D, 1D}.ToArray();

            var rnd = new Random();

            expectedOut = new[] { 0D };

            for (var n = 0; error >= ErrorThresh; n++)
            {
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Iterations: {0} - Error Thresh: {1} - Current Error: {2}", n, ErrorThresh, error );
                var errorList = new List<double>();
                for (var x = 1; x <= maxLength; x++)
                {
                    var badArray = new double[x];
                    for (var y = 0; y < x; y++)
                    {
                        badArray[y] = consanents[rnd.Next(0, 21)];
                    }
                    service.Train(badArray, expectedOut);
                    errorList.Add(service.LastError);
                }
                error = Math.Sqrt(errorList.Sum(e => e * e));
            }

            Console.WriteLine("Ready! Give me words to try and guess!");

        }
    }
}
