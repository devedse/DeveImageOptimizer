using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DeveImageOptimizer.ConsoleApp
{
    class FileProcessedListener : IFilesProcessedListener
    {
        public void AddProcessedFile(OptimizedFileResult optimizedFileResult)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {optimizedFileResult}");
        }
    }
}
