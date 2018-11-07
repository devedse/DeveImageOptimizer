using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
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
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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

            var testName = nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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

            var testName = nameof(CorrectlyOptimizesCompleteDirectoryAndSkipsFileIfTheyHaveTheSameHashAndAreBothAlreadyOptimized);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimizeWithTwoOfTheSameImage", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(2, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));                
            }
        }

        [SkippableFact]
        public async Task ProcessSampleDirInParallel()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var testName = nameof(ProcessSampleDirInParallel);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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

            var testName = nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

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

            var testName = nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel);
            var fileNameFileProcessedStateRememberer = $"{testName}.txt";

            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false, fileNameFileProcessedStateRememberer);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize, TestConstants.MaxDegreeOfParallelism)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        private static string PrepareTestOptimizeDir(string dirToOptimize, string fileNameFileProcessedStateRememberer, string testName)
        {
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var sourceSampleDirToOptimize = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", dirToOptimize);
            var sampleDirToOptimize = Path.Combine(tempfortestdir, testName);

            if (Directory.Exists(sampleDirToOptimize))
            {
                Directory.Delete(sampleDirToOptimize, true);
            }
            Directory.CreateDirectory(sampleDirToOptimize);
            foreach (var file in Directory.GetFiles(sourceSampleDirToOptimize))
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(sampleDirToOptimize, fileName);
                File.Copy(file, destPath);
            }

            //Delete optimized file storage
            var filePathOfRemembererStorage = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, fileNameFileProcessedStateRememberer);
            File.Delete(filePathOfRemembererStorage);
            return sampleDirToOptimize;
        }
    }
}
