using System;
using System.IO;
using EdGps.Core;

namespace EdGps
{
    public class Program
    {
        public static void Main(string[] args) {
            Console.Write("Please specify the directory where E:D Journal logs are kept. You may drag & drop if possible");
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
        }
    }
}
