using DeveCoolLib.ProcessAsTask;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationStepResult
    {
        public ProcessResults ProcessResults { get; }
        public string OutputPath { get; }

        public ImageOptimizationStepResult(ProcessResults processResults, string outputPath)
        {
            ProcessResults = processResults;
            OutputPath = outputPath;
        }
    }
}
