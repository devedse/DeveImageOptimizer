using DeveImageOptimizer.State;
using System;
using System.Threading;

namespace DeveImageOptimizer.ConsoleApp
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        public void OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} File Optimized: {optimizableFile}");
        }

        public void TotalFileCountDiscovered(int count)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Total file count: {count}");
        }
    }
}
