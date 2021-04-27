using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOptimization;
using System;

namespace DeveImageOptimizer.FileProcessing
{
    public class DeveImageOptimizerConfiguration
    {
        private static char s = System.IO.Path.DirectorySeparatorChar;
        public string FileOptimizerPath { get; set; } = Environment.GetEnvironmentVariable("FILEOPTIMIZERPATH") ?? $"C:{s}Program Files{s}FileOptimizer{s}FileOptimizer64.exe";
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
        /// By default this program uses the synchronisationcontext for progress reporting. However in some scenario's you want to make sure that progress reporting
        /// completes before the image optimization process is finished. That's when you should put this to true.
        /// </summary>
        public bool AwaitProgressReporting { get; set; } = false;

        /// <summary>
        /// This settings ensures the DeveImageOptimizer doesn't make use of FileOptimizerFull anymore. It calls the tool .exe files directly.
        /// </summary>
        public bool CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer { get; set; } = true;

        /// <summary>
        /// This setting determines if logs from internal tools will be forwarded to the console log
        /// (Only works when UseNewDeveImageOptimizer is enabled)
        /// </summary>
        public bool ForwardOptimizerToolLogsToConsole { get; set; } = false;

        /// <summary>
        /// Configure how strong the image compression should be. Only the levels 'Maximum' and 'Placebo' will 
        /// </summary>
        public ImageOptimizationLevel ImageOptimizationLevel { get; set; } = ImageOptimizationLevel.Maximum;
    }
}
