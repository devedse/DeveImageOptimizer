﻿using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using System;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var dirrr = @"C:\TheCFolder\TestMapKanWeg";

            var fileProcessedListener = new ConsoleProgressReporter();

            var rememberer = new FileProcessedStateRememberer(true);
            var dirRememberer = new DirProcessedStateRememberer(true);

            var config = new DeveImageOptimizerConfiguration()
            {
                ExecuteImageOptimizationParallel = false,
                UseNewDeveImageOptimizer = true
            };

            var fp = new DeveImageOptimizerProcessor(config, fileProcessedListener, rememberer, dirRememberer);

            await fp.ProcessDirectory(dirrr);
            Console.WriteLine("Application done press any key to exit");
            Console.ReadKey();
        }
    }
}
