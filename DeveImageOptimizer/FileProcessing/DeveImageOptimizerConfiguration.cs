using DeveImageOptimizer.Helpers;

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
        public int MaxDegreeOfParallelism { get; set; } = 4;

        public int LogLevel { get; set; } = 2;
    }
}
