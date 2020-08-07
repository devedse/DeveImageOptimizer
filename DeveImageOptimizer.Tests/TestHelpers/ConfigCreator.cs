using DeveImageOptimizer.FileProcessing;

namespace DeveImageOptimizer.Tests.TestHelpers
{
    public static class ConfigCreator
    {
        public static DeveImageOptimizerConfiguration CreateTestConfig(bool parallel)
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var config = new DeveImageOptimizerConfiguration()
            {
                FileOptimizerPath = fileOptimizerPath,
                MaxDegreeOfParallelism = 8,
                ExecuteImageOptimizationParallel = parallel,
                HideFileOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                LogLevel = 4
            };
            return config;
        }
    }
}
