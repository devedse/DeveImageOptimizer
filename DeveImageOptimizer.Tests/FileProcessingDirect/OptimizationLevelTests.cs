using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.FileProcessingDirect;
using DeveImageOptimizer.Tests.TestHelpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessingFileOptimizer
{
    public class OptimizationLevelTests
    {
        [SkippableFact]
        public async Task WorksWithMultipleOptimizationLevels()
        {
            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(WorksWithMultipleOptimizationLevels)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);


            var optimizationLevels = new List<ImageOptimizationLevel>()
            {
                ImageOptimizationLevel.SuperFast,
                ImageOptimizationLevel.Fast,
                ImageOptimizationLevel.Normal,
                ImageOptimizationLevel.Maximum,
                ImageOptimizationLevel.Placebo
            };

            var durations = new Dictionary<ImageOptimizationLevel, string>();

            foreach (var optimizationLevel in optimizationLevels)
            {
                var config = ConfigCreator.CreateTestConfig(false, optimizationLevel);

                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var w = Stopwatch.StartNew();
                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();
                w.Stop();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    //Assert.True(result.OriginalSize > result.OptimizedSize);
                }

                var totalSize = results.Sum(t => t.OptimizedSize);

                durations.Add(optimizationLevel, $"Duration: {w.Elapsed}, Size: {totalSize}");
            }
        }


        [SkippableFact]
        public async Task WorksWithMultipleOptimizationLevelsInverse()
        {
            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(WorksWithMultipleOptimizationLevelsInverse)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);


            var optimizationLevels = new List<ImageOptimizationLevel>()
            {
                ImageOptimizationLevel.Placebo,
                ImageOptimizationLevel.Maximum,
                ImageOptimizationLevel.Normal,
                ImageOptimizationLevel.Fast,
                ImageOptimizationLevel.SuperFast
            };

            var durations = new Dictionary<ImageOptimizationLevel, string>();

            foreach (var optimizationLevel in optimizationLevels)
            {
                var config = ConfigCreator.CreateTestConfig(false, optimizationLevel);

                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var w = Stopwatch.StartNew();
                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();
                w.Stop();

                if (optimizationLevel == ImageOptimizationLevel.Placebo)
                {
                    Assert.Equal(4, results.Count);
                    foreach (var result in results)
                    {
                        Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                        Assert.True(result.OriginalSize > result.OptimizedSize);
                    }
                }
                else
                {
                    Assert.Equal(4, results.Count);
                    foreach (var result in results)
                    {
                        Assert.Equal(OptimizationResult.Skipped, result.OptimizationResult);
                        Assert.True(result.OriginalSize == result.OptimizedSize);
                    }
                }

                var totalSize = results.Sum(t => t.OptimizedSize);

                durations.Add(optimizationLevel, $"Duration: {w.Elapsed}, Size: {totalSize}");
            }
        }
    }
}
