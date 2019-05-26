using System;
using EdGps.Core;

namespace EdGps
{
    public class Program
    {
        public static void Main(string[] args) {
            Console.Write("E:D Journal Log Directory :");
            var directoryPath = Parser.ParseDirectoryPath(Console.ReadLine());
        }
    }
}
