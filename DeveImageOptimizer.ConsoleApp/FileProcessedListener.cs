using DeveImageOptimizer.State;
using System;
using System.Threading;

namespace DeveImageOptimizer.ConsoleApp
{
    public class FileProcessedListener : IFilesProcessedListener
    {
        public void AddProcessedFile(OptimizedFileResult optimizedFileResult)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {optimizedFileResult}");
        }
    }
}
