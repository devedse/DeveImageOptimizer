using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.ImageOptimization;

namespace DeveImageOptimizer.Tests.TestHelpers
{
    public static class ConfigCreator
    {
        public static DeveImageOptimizerConfiguration CreateTestConfig(bool parallel = false, ImageOptimizationLevel imageOptimizationLevel = ImageOptimizationLevel.SuperFast, bool directMode = true)
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
                CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer = directMode
            };
            return config;
        }
    }
}
