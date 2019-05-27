using System;
using System.IO;
using System.Threading.Tasks;
using EdGps.Core;

namespace EdGps
{
    public class Program
    {
        private Gps _gps = null;

        public static void Main(string[] args) {
            if (!Directory.Exists(Directories.SystemDir)) Directory.CreateDirectory(Directories.SystemDir);

            //Check if user has already set a path
            // NOTE: This should be changed to some common config file, like a config.INI or something for future use
            string edLogDirectoryFile = Path.Join(System.AppContext.BaseDirectory, "userdirectory.txt");

            string directoryPath = null;
            string exittext = "\nPress any key to exit...";

            try
            {
                if (File.Exists(edLogDirectoryFile))
                {
                    directoryPath = File.ReadAllText(edLogDirectoryFile);
                }
                else
                {
                    Console.WriteLine("\n\nE:D Journal Log Directory (e.g. C:\\Users\\<username>\\Saved Games\\Frontier Developments\\Elite Dangerous) :\n");
                    directoryPath = Console.ReadLine()
                        .Replace('\'', ' ')
                        .Replace('&', ' ')
                        .Replace('"', ' ')
                        .TrimStart(' ')
                        .TrimEnd(' ');

                    if (directoryPath.Length <= 4)
                    {
                        Console.WriteLine("Please specify the directory where E:D Journal logs are kept.");
                        Console.WriteLine(exittext);
                        Console.ReadKey();
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(exittext);
                Console.ReadKey();
                return;
            }

            //Some user error handling
            try
            {
                if (!(Directory.Exists(directoryPath)))
                {
                    throw new Exception("\nError: Directory does not exist.\n");
                }
                if (Directory.GetFiles(directoryPath).Length <= 0)
                {
                    throw new Exception("\nError: Directory has no files.\n");
                }
                bool islogfile = false;
                foreach (string file in Directory.GetFiles(directoryPath))
                {
                    if (file.EndsWith(".log"))
                    {
                        islogfile = true;
                    }
                }
                if (!islogfile)
                {
                    throw new Exception("\nError: Directory has no *.log files.\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine("\nPlease retry...");
                Main(null);
            }

            //Save Path if legit
            try
            {
                File.WriteAllText(edLogDirectoryFile, directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(exittext);
                Console.ReadKey();
                return;
            }

            // If everything's okay, move on

            new Program().StartAsync(directoryPath).GetAwaiter().GetResult();
        }

        public async Task StartAsync(string directoryPath) {
            _gps = new Gps(directoryPath);
            _gps.Start();
            
            await Task.Delay(-1);
        }
    }
}
