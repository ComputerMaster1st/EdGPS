using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EdGps.Core;

namespace EdGps
{
    public class Program
    {
        private Gps _gps = null;
        private static bool firstTry = true;

        public static void Main() {
            Console.Title = "Elite: Dangerous | Galactic Positioning System";
            if (!Directory.Exists(Directories.SystemDir)) Directory.CreateDirectory(Directories.SystemDir);

            //Check if user has already set a path
            var config = Config.LoadOrCreate();
            var exittext = "\nPress any key to exit...";
            string directoryPath;

            try {
                if (!string.IsNullOrEmpty(config.JournalPath)) directoryPath = config.JournalPath;
                else {
                    string os = System.Runtime.InteropServices.RuntimeInformation.OSDescription.ToString().ToLower();

                    if (os.Contains("windows") && firstTry){
                        string username = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        username = username.Substring(username.LastIndexOf("\\"));
                        username = username.Replace("\\", "");
                        directoryPath = "C:\\Users\\" + username + "\\Saved Games\\Frontier Developments\\Elite Dangerous";
                    }else{
                        Console.WriteLine("E:D Journal Log Directory couldn't be determined automatically, please enter it manually");
                        Console.WriteLine("(e.g. C:\\Users\\<username>\\Saved Games\\Frontier Developments\\Elite Dangerous):\n");
                        directoryPath = Parser.SanitizeDirectory(Console.ReadLine());

                        if (directoryPath.Length <= 4)
                        {
                            Console.WriteLine("Please specify the directory where E:D Journal logs are kept.");
                            Console.WriteLine(exittext);
                            Console.ReadKey();
                            return;
                        }
                    }  
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(exittext);
                Console.ReadKey();
                return;
            }

            //Some user error handling
            try {
                if (!Directory.Exists(directoryPath)) throw new Exception("\nError: Directory does not exist.\n");
                if (Directory.GetFiles(directoryPath).Length <= 0) throw new Exception("\nError: Directory has no files.\n");

                var islogfile = false;
                foreach (string file in Directory.GetFiles(directoryPath))
                    if (file.EndsWith(".log")) islogfile = true;

                if (!islogfile) throw new Exception("\nError: Directory has no *.log files.\n");
            } catch (Exception e) {
                firstTry = false;
                Console.WriteLine("Error: " + e);
                Console.WriteLine("\nPlease retry...");
                Console.Clear();
                Main();
            }

            //Save Path if legit
            try  {
                config.SetJournalPath(directoryPath);
            } catch (Exception e) {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(exittext);
                Console.ReadKey();
                return;
            }

            // If everything's okay, move on
            var build = false;
            if (new DirectoryInfo(Directories.SystemDir).EnumerateFiles().Count() < 1) build = true;
            new Program().StartAsync(config, build).GetAwaiter().GetResult();
        }

        public async Task StartAsync(Config config, bool rebuild = false) {
            _gps = new Gps(config);
            _gps.Start(rebuild);
            await Task.Delay(-1);
        }
    }
}
