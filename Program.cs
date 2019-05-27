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

            Console.WriteLine("Please specify the directory where E:D Journal logs are kept. You may drag & drop if possible");
            var directoryPath = "";
            var input = "";

            while (true) {
                Console.Write("Path:");
                input = Console.ReadLine();

                if (input.Length < 1) continue;

                directoryPath = Parser.ParseDirectoryPath(input);
                if (Directory.Exists(directoryPath)) break;
                
                Console.WriteLine("Unable to find directory! Please try again.");
            }

            new Program().StartAsync(directoryPath).GetAwaiter().GetResult();
        }

        public async Task StartAsync(string directoryPath) {
            _gps = new Gps(directoryPath);
            
            await _gps.StartAsync();
            await Task.Delay(-1);
        }
    }
}
