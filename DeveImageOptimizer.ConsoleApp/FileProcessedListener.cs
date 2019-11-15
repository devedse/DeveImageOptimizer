using DeveImageOptimizer.State;
using System;
using System.Threading;

namespace DeveImageOptimizer.ConsoleApp
{
    public class FileProcessedListener : IFilesProcessedListener
    {
        public void AddProcessedFile(OptimizableFile optimizedFileResult)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {optimizedFileResult}");
        }
    }
}
