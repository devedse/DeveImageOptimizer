using DeveImageOptimizer.State;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ConsoleApp
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        public Task OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"File Optimized: {optimizableFile}");
            return Task.CompletedTask;
        }

        public Task TotalFileCountDiscovered(int count)
        {
            Console.WriteLine($"Total file count: {count}");
            return Task.CompletedTask;
        }
    }
}
