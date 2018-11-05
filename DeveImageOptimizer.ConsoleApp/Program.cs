using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.State;
using System;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            Console.WriteLine("Application done press any key to exit");
            Console.ReadKey();
        }


        public static async Task MainAsync(string[] args)
        {
            var dirrr = @"C:\KanWeg";

            var fop = new FileOptimizerProcessor(@"C:\Program Files\FileOptimizer\FileOptimizer64.exe", "Temp", true);
            var fileProcessedListener = new FileProcessedListener();
            var rememberer = new FileProcessedStateRememberer(true);
            var fp = new FileProcessor(fop, fileProcessedListener, rememberer);

            await fp.ProcessDirectoryParallel(dirrr, 4);
        }
    }
}
