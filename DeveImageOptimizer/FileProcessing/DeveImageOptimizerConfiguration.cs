﻿using DeveImageOptimizer.Helpers;
using System;

namespace DeveImageOptimizer.FileProcessing
{
    public class DeveImageOptimizerConfiguration
    {
        public string FileOptimizerPath { get; set; } = @"C:\Program Files\FileOptimizer\FileOptimizer64.exe";
        public string TempDirectory { get; set; } = FolderHelperMethods.TempDirectory;

        public bool HideFileOptimizerWindow { get; set; } = true;

        public string FailedFilesDirectory { get; set; } = FolderHelperMethods.FailedFilesDirectory;
        public bool SaveFailedFiles { get; set; } = false;

        public bool ExecuteImageOptimizationParallel { get; set; } = true;
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// The log level passed to FileOptimizerFull
        /// </summary>
        public int LogLevel { get; set; } = 2;

        /// <summary>
        /// This loops through all the files first to ensure we get a total filecount before starting the optimization
        /// </summary>
        public bool DetermineCountFilesBeforehand { get; set; } = false;

        /// <summary>
        /// By default it uses the synchronisationcontext for progress reporting. However in some scenario's you want to make sure that progress reporting
        /// completes before the image optimization process is finished. That's when you should put this to true.
        /// </summary>
        public bool AwaitProgressReporting { get; set; } = false;
    }
}
