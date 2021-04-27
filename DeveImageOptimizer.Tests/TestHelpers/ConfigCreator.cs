using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.ImageOptimization;

namespace DeveImageOptimizer.Tests.TestHelpers
{
    public static class ConfigCreator
    {
        public static DeveImageOptimizerConfiguration CreateTestConfig(bool parallel, ImageOptimizationLevel imageOptimizationLevel = ImageOptimizationLevel.Maximum)
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var config = new DeveImageOptimizerConfiguration()
            {
                FileOptimizerPath = fileOptimizerPath,
                MaxDegreeOfParallelism = 8,
                ExecuteImageOptimizationParallel = parallel,
                HideOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                LogLevel = 4,
                ImageOptimizationLevel = imageOptimizationLevel,
                CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer = true
            };
            return config;
        }
    }
}
