using System.Diagnostics;

namespace FocusCoding
{
    internal class Program
    {

        // Hosts file location
        private static readonly string hostsPath =
            @"C:\Windows\System32\drivers\etc\hosts";

        // List of sites to block
        private static readonly string[] blockedSites =
        {
        "www.chatgpt.com",
        "www.youtube.com"
        };

        // List of processes to check
        private static readonly string[] processes =
        {
            "Code" // VSCode
        };


        static void Main(string[] args)
        {
            Console.WriteLine(ShouldBlock());
        }

        private static bool IsRunning(string process)
        {
            return Process.GetProcessesByName(process).Any();
        }

        private static bool ShouldBlock()
        {
            for(int i = 0; i < processes.Length; i++)
            {
                if (IsRunning(processes[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
