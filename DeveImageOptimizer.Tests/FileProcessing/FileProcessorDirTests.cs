using CodeAssassin;
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
    public class FileProcessorDirTests
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task ProcessSampleDirInParallel()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(ProcessSampleDirInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesReadonlyAndBlockedFilesInDirectory()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesReadonlyAndBlockedFilesInDirectory)}";
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";
            var fileNameDirProcessedStateRememberer = $"{testName}-dir.txt";

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("DirWithReadonlyFile", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            var blockedJpg = Path.Combine(sampleDirToOptimize, "BlockedJpg.jpg");
            var readonlyJpg = Path.Combine(sampleDirToOptimize, "ReadOnlyJpg.jpg");

            //Prepare files
            using (var zoneIdentifier = new ZoneIdentifier(blockedJpg))
            {
                zoneIdentifier.Zone = UrlZone.Internet;
            }
            new FileInfo(readonlyJpg).IsReadOnly = true;

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                }
            }
        }
    }
}
