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
        "chatgpt.com",
        "www.chatgpt.com",
        "youtube.com",
        "www.youtube.com"
        };

        // List of processes to check
        private static readonly string[] processes =
        {
            "Code" // VSCode
        };

        private static readonly string newIP = "127.0.0.1";


        static void Main(string[] args)
        {
            Console.WriteLine("Starting FocusCoding Blocker");
            bool currentlyBlocking = false;
            while (true)
            {
                try {
                    if (!currentlyBlocking && ShouldBlock())
                    {
                        currentlyBlocking = true;
                        Console.WriteLine("IDE Running, Blocking Sites.");
                        BlockSites();
                    }
                    else if (currentlyBlocking && !ShouldBlock())
                    {
                        currentlyBlocking = false;
                        Console.WriteLine("No IDEs Running, Unblocking Sites.");
                        UnblockSites();
                    }
                } 
                catch(Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                Thread.Sleep(15000);    // 30000 = 30 seconds
            }
        }

        private static bool IsRunning(string process)
        {
            return Process.GetProcessesByName(process).Any();
        }

        private static bool ShouldBlock()
        {
            foreach (string p in processes)
            {
                if (IsRunning(p))
                {
                    return true;
                }
            }
            return false;
        }

        private static void BlockSites()
        {
            var content = File.ReadAllText(hostsPath);
            var newLines = new List<string>();

            foreach (var site in blockedSites)
            {
                string entry = $"{newIP} {site}";
                if (!content.Contains(entry))
                    newLines.Add(entry);
            }

            if (newLines.Count > 0)
                File.AppendAllLines(hostsPath, newLines);
        }


        private static void UnblockSites()
        {
            var lines = File.ReadAllLines(hostsPath)
                .Where(line => !blockedSites.Any(site => line.Contains(site)))
                .ToList();

            File.WriteAllLines(hostsPath, lines);
        }
    }
}
