using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessing
{
    public class TestImageOptimizationLevel
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesWithOptimizationLevel()
        {
            var config = ConfigCreator.CreateTestConfig(false);
            config.ImageOptimizationLevel = ImageOptimizationLevel.SuperFast;

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesWithOptimizationLevel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            long totalSizeOriginal = 0;
            long totalSize1 = 0;
            long totalSize2 = 0;
            long totalSize3 = 0;

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                    Assert.Equal(ImageOptimizationLevel.SuperFast, result.ImageOptimizationLevel);
                }

                totalSizeOriginal = results.Sum(t => t.OriginalSize);
                totalSize1 = results.Sum(t => t.OptimizedSize);
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Skipped, result.OptimizationResult);
                    Assert.True(result.OriginalSize == result.OptimizedSize);
                    Assert.Null(result.ImageOptimizationLevel);
                }

                totalSize2 = results.Sum(t => t.OptimizedSize);
            }

            config.ImageOptimizationLevel = ImageOptimizationLevel.Maximum;

            //Optimize third time with better optimization level
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.Equal(ImageOptimizationLevel.Maximum, result.ImageOptimizationLevel);
                }

                totalSize3 = results.Sum(t => t.OptimizedSize);
            }

            Assert.True(totalSizeOriginal > totalSize1);
            Assert.True(totalSizeOriginal > totalSize2);
            Assert.True(totalSizeOriginal > totalSize3);
            Assert.True(totalSize1 == totalSize2);
            Assert.True(totalSize2 > totalSize3);
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesWithOptimizationLevelWithDb()
        {
            var config = ConfigCreator.CreateTestConfig(false);
            config.ImageOptimizationLevel = ImageOptimizationLevel.SuperFast;

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesWithOptimizationLevel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.db");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            long totalSizeOriginal = 0;
            long totalSize1 = 0;
            long totalSize2 = 0;
            long totalSize3 = 0;

            //Optimize first time                
            {
                var rememberer = new SqlProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                    Assert.Equal(ImageOptimizationLevel.SuperFast, result.ImageOptimizationLevel);
                }

                totalSizeOriginal = results.Sum(t => t.OriginalSize);
                totalSize1 = results.Sum(t => t.OptimizedSize);
            }

            //Optimize second time
            {
                var rememberer = new SqlProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Skipped, result.OptimizationResult);
                    Assert.True(result.OriginalSize == result.OptimizedSize);
                    Assert.Null(result.ImageOptimizationLevel);
                }

                totalSize2 = results.Sum(t => t.OptimizedSize);
            }

            config.ImageOptimizationLevel = ImageOptimizationLevel.Maximum;

            //Optimize third time with better optimization level
            {
                var rememberer = new SqlProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.Equal(ImageOptimizationLevel.Maximum, result.ImageOptimizationLevel);
                }

                totalSize3 = results.Sum(t => t.OptimizedSize);
            }

            Assert.True(totalSizeOriginal > totalSize1);
            Assert.True(totalSizeOriginal > totalSize2);
            Assert.True(totalSizeOriginal > totalSize3);
            Assert.True(totalSize1 == totalSize2);
            Assert.True(totalSize2 > totalSize3);
        }
    }
}
