using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI
{
    public class Reader
    {
        public static void Run()
        {
            var dir = Directory.GetCurrentDirectory().Replace("bin\\Debug", "dictionary.txt");
            var reader = new System.IO.StreamReader(dir);

            var wordList = new List<string>();
            

            while (reader.Peek() != 0)
            {
                var nextLine = reader.ReadLine().Trim();
                if (nextLine == null) break;
                wordList.Add(nextLine);
            }

            Console.WriteLine(dir);

        }
    }
}
