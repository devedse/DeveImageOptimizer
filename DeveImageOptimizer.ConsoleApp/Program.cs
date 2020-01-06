using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
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

            var aaa = FolderHelperMethods.Internal_AppDataFolder.Value;


            var dirrr = @"C:\KanWeg";

            var fileProcessedListener = new ConsoleProgressReporter();

            var rememberer = new FileProcessedStateRememberer(true);
            var dirRememberer = new DirProcessedStateRememberer(true);

            var fp = new DeveImageOptimizerProcessor(new DeveImageOptimizerConfiguration(), fileProcessedListener, rememberer, dirRememberer);

            await fp.ProcessDirectoryParallel(dirrr, 4);
        }
    }
}
