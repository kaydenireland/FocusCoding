using System.Diagnostics;

namespace FocusCoding
{
    internal class Program
    {

        private static readonly string hostsPath = @"C:\Windows\System32\drivers\etc\hosts";

        private static readonly string[] blockedSites =
        {
        "chatgpt.com",
        "claude.ai",
        "chat.openai.com",
        "copilot.microsoft.com",
        "bard.google.com",
        "gemini.google.com",
        "grok.com",

        "youtube.com",
        "twitch.tv",

        "hulu.com",
        "netflix.com",

        "instagram.com",
        "tiktok.com",
        "facebook.com",
        "x.com",
        "reddit.com",
        "snapchat.com",
        "discord.com"
        };

        private static readonly string[] processes =
        {
            "Code", // VSCode
            "devenv", // Visual Studio

            "notepad++",
            "Brackets",
            "atom",
            "studio64",
            "Xcode",

            "Unity",
            "UnrealEditor",

            "eclipse",
            "netbeans",
            "netbeans64",

            "spyder",
            "thonny",

            "idea64",
            "pycharm64",
            "clion64",
            "rider64",
            "webstorm64",
            "phpstorm64",
            "rubymine64"

        };

        private static readonly string newIP = "0.0.0.0";


        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (_, __) => UnblockSites();
            Console.WriteLine("Starting FocusCoding Blocker");
            bool currentlyBlocking = false;

            try
            {
                while (true)
                {
                    try
                    {
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
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    Thread.Sleep(15000);    // 15000 = 15 seconds
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error: " + ex.Message);
                UnblockSites();
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
                {
                    newLines.Add(entry);
                }

                entry = $"{newIP} www.{site}";
                if (!content.Contains(entry))
                {
                    newLines.Add(entry);
                }
            }

            if (newLines.Count > 0)
            {
                File.AppendAllLines(hostsPath, newLines);
            }
            FlushDNS();
        }


        private static void UnblockSites()
        {
            var lines = File.ReadAllLines(hostsPath)
                .Where(line => !blockedSites.Any(site => line.Contains(site)))
                .ToList();

            File.WriteAllLines(hostsPath, lines);
            FlushDNS();
        }

        private static void FlushDNS()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ipconfig",
                Arguments = "/flushdns",
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

    }
}
