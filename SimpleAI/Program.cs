using System;

namespace SimpleAI
{
    internal class Program
    {
        private static void Main()
        {
            Reader.Run();
            Console.WriteLine("Press any key to end the program...");
            Console.ReadKey();
        }
    }
}
