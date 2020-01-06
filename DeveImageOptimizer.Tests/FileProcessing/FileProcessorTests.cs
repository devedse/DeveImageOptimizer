using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessing
{
    public class FileProcessorTests
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime()
        {
            var config = ConfigCreator.CreateTestConfig(false);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

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
                }
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
                }
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles()
        {
            var config = ConfigCreator.CreateTestConfig(false);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndSkipsFileIfTheyHaveTheSameHashAndAreBothAlreadyOptimized()
        {
            var config = ConfigCreator.CreateTestConfig(false);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndSkipsFileIfTheyHaveTheSameHashAndAreBothAlreadyOptimized)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeWithTwoOfTheSameImage", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(2, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
            }
        }

        [SkippableFact]
        public async Task ProcessSampleDirInParallel()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(ProcessSampleDirInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                }
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                }
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Skipped, result.OptimizationResult);
                    Assert.True(result.OriginalSize == result.OptimizedSize);
                }
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }
    }
}
