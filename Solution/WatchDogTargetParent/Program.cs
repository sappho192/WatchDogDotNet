using System;
using System.Diagnostics;
using System.Threading;

namespace WatchDogTargetParent
{
    class Program
    {
        private static Timer schedule;
        private static uint count = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Booting child program");
            Process childProcess = Process.Start("WatchDogTargetChild.exe");

            // Parent work
            schedule = new Timer(work, null, 0, 1000);

            // Booting watchdog
            var PID = Process.GetCurrentProcess().Id;
            Process watchdogProcess = Process.Start("WatchDogMain.exe", $"{PID}");

            Console.ReadLine();
        }

        private static void work(object state)
        {
            Console.WriteLine($"Parent works...[{count++}]");
        }
    }
}
