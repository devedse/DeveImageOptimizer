﻿using DeveImageOptimizer.State;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ConsoleApp
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        public Task OptimizableFileProgressUpdated(OptimizableFile optimizableFile)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} File Optimized: {optimizableFile}");
            return Task.CompletedTask;
        }

        public Task TotalFileCountDiscovered(int count)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Total file count: {count}");
            return Task.CompletedTask;
        }
    }
}
