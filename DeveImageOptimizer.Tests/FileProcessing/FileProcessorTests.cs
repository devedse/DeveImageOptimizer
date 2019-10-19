using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.TestHelpers;
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
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

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
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

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
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndSkipsFileIfTheyHaveTheSameHashAndAreBothAlreadyOptimized()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndSkipsFileIfTheyHaveTheSameHashAndAreBothAlreadyOptimized)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeWithTwoOfTheSameImage", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(2, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
            }
        }

        [SkippableFact]
        public async Task ProcessSampleDirInParallel()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(ProcessSampleDirInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

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
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

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
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

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
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(true, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }
    }
}
