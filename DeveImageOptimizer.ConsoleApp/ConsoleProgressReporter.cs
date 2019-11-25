using DeveImageOptimizer.State;
using System;
using System.Threading;

namespace DeveImageOptimizer.ConsoleApp
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        public void OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {optimizableFile}");
        }
    }
}
