using System.Collections.Generic;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationPlanResult
    {
        public bool Success { get; }
        public string OutputPath { get; }
        public IList<string> ErrorsLog { get; }
        public IList<string> OutputLog { get; }

        public ImageOptimizationPlanResult(bool success, string outputPath, IList<string> errorsLog, IList<string> outputLog)
        {
            Success = success;
            OutputPath = outputPath;
            ErrorsLog = errorsLog;
            OutputLog = outputLog;
        }
    }
}
