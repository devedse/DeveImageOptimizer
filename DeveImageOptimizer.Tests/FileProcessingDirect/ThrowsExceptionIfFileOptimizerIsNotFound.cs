using DeveImageOptimizer.Exceptions;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.TestHelpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessingDirect
{
    public class ThrowsExceptionIfFileOptimizerIsNotFound
    {
        [Fact]
        public async Task ThrowsTheExceptionInParallelOptimization()
        {
            var config = new DeveImageOptimizerConfiguration()
            {
                FileOptimizerPath = @"C:\brokenpathfile.exe",
                HideOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer = true,
                LogLevel = 4,
                ImageOptimizationLevel = ImageOptimization.ImageOptimizationLevel.SuperFast,
                ExecuteImageOptimizationParallel = true
            };

            var testName = $"{nameof(ThrowsExceptionIfFileOptimizerIsNotFound)}_{nameof(ThrowsTheExceptionInParallelOptimization)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
            var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
            var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

            try
            {
                await fp.ProcessDirectory(sampleDirToOptimize);

                Assert.True(false);
            }
            catch (Exception ex) when (ex is FileOptimizerNotFoundException || (ex as AggregateException)?.InnerExceptions?.OfType<FileOptimizerNotFoundException>()?.Any() == true)
            {
                Assert.NotNull(ex);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async Task ThrowsTheExceptionInNonParallelOptimization()
        {
            var config = new DeveImageOptimizerConfiguration()
            {
                FileOptimizerPath = @"C:\brokenpathfile.exe",
                HideOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer = true,
                LogLevel = 4,
                ImageOptimizationLevel = ImageOptimization.ImageOptimizationLevel.SuperFast,
                ExecuteImageOptimizationParallel = false
            };

            var testName = $"{nameof(ThrowsExceptionIfFileOptimizerIsNotFound)}_{nameof(ThrowsTheExceptionInNonParallelOptimization)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
            var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
            var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

            try
            {
                await fp.ProcessDirectory(sampleDirToOptimize);

                Assert.True(false);
            }
            catch (Exception ex) when (ex is FileOptimizerNotFoundException || (ex as AggregateException)?.InnerExceptions?.OfType<FileOptimizerNotFoundException>()?.Any() == true)
            {
                Assert.NotNull(ex);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }
    }
}
