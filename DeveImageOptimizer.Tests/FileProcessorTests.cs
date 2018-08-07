using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class FileProcessorTests
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();
            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimize");

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false);
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
                var rememberer = new FileProcessedStateRememberer(false);
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
            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg");

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Skipped));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [Fact]
        public async Task Testje()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();
            string sampleDirToOptimize = PrepareTestOptimizeDir("SampleDirToOptimize");

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, TestConstants.ShouldShowFileOptimizerWindow);
                var rememberer = new FileProcessedStateRememberer(false);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectoryParallel(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.Equal(OptimizationResult.Success, result.OptimizationResult);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                }
            }
        }

        private static string PrepareTestOptimizeDir(string dirToOptimize)
        {
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var sourceSampleDirToOptimize = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", dirToOptimize);
            var sampleDirToOptimize = Path.Combine(tempfortestdir, dirToOptimize);

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
            var filePathOfRemembererStorage = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, FileProcessedStateRememberer.FileNameHashesStorage);
            File.Delete(filePathOfRemembererStorage);
            return sampleDirToOptimize;
        }
    }
}
