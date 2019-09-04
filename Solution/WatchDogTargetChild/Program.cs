using System;
using System.Threading;

namespace WatchDogTargetChild
{
    class Program
    {
        private static Timer schedule = new Timer(work, null, 0, 1000);
        private static uint count = 0;

        static void Main(string[] args)
        {
            Console.ReadLine();
        }

        private static void work(object state)
        {
            Console.WriteLine($"Child works...[{count++}]");
        }
    }
}
