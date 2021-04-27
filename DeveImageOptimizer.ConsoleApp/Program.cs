using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var dirrr = @"C:\TheCFolder\TestMapKanWeg";
            var dirrr = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var fileProcessedListener = new ConsoleProgressReporter();

            var rememberer = new FileProcessedStateRememberer(true);
            var dirRememberer = new DirProcessedStateRememberer(true);

            var config = new DeveImageOptimizerConfiguration()
            {
                ExecuteImageOptimizationParallel = false,
                CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer = true,
                ForwardOptimizerToolLogsToConsole = false,
                ImageOptimizationLevel = ImageOptimization.ImageOptimizationLevel.Placebo
                //FileOptimizerPath = Assembly.GetEntryAssembly().Location
            };

            var fp = new DeveImageOptimizerProcessor(config, fileProcessedListener, rememberer, dirRememberer);

            var filesProcessedIEnumerable = await fp.ProcessDirectory(dirrr);
            var filesProcessed = filesProcessedIEnumerable.ToList();
            Console.WriteLine();
            Console.WriteLine("Application completed, stats:");
            for (int i = 0; i < filesProcessed.Count; i++)
            {
                var cur = filesProcessed[i];
                Console.WriteLine($"{i + 1}/{filesProcessed.Count}: {Path.GetFileName(cur.Path)} ({cur.OptimizationResult}) {cur.OriginalSize} --> {cur.OptimizedSize} (Reduced by: {cur.OriginalSize - cur.OptimizedSize} {{{Math.Round((double)cur.OptimizedSize / (double)cur.OriginalSize * 100.0, 1)}%}}) in {Math.Round(cur.Duration.TotalSeconds, 1)} seconds");
            }
        }
    }
}
