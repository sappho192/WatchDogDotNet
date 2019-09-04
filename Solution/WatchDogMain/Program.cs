using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;

namespace WatchDogMain
{
    class Program
    {
        private static Timer schedule;
        private static uint count = 0;
        private static int targetPID;

        static void Main(string[] args)
        {
            Console.WriteLine($"arg count: {args.Length}");
            if (args.Length != 1)
            {
                Console.WriteLine($"Watchdog should know your process ID with ONE argument");
            }
            else
            {
                targetPID = int.Parse(args[0]);
                if (!ProcessExists(targetPID))
                {
                    Console.WriteLine($"Process {targetPID} doesn't exist");
                }
                else
                {
                    Console.WriteLine($"Found process {targetPID}");
                    schedule = new Timer(work, null, 0, 1000);
                }
            }
            Console.ReadLine();
        }

        private static void KillAllProcessesSpawnedBy(uint parentProcessId)
        {
            // NOTE: Process Ids are reused!
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    uint childProcessId = (uint)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {
                        KillAllProcessesSpawnedBy(childProcessId);

                        Process childProcess = Process.GetProcessById((int)childProcessId);
                        childProcess.Kill();
                    }
                }
            }
        }

        private static bool ProcessExists(int id)
        {
            if(Process.GetProcesses().Any(x => x.Id == id))
            {
                return Process.GetProcessById(id).Responding;
            }
            return false;
        }

        private static void work(object state)
        {
            Console.WriteLine($"WatchDog works...[{count++}]");
            if(!ProcessExists(targetPID))
            {
                Console.WriteLine($"Target process is dead. Killing child process");
                KillAllProcessesSpawnedBy((uint)targetPID);
                schedule.Dispose();
                Environment.Exit(0);
            } else
            {
                Console.WriteLine($"Target process is alive");
            }
        }
    }
}
