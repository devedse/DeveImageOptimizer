using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using DeveImageOptimizer.Tests.ExternalTools;
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
            var config = ConfigCreator.CreateTestConfig(false);

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFiles)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task ProcessSampleDirInParallel()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(ProcessSampleDirInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

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

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTimeInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimize", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
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
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesCompleteDirectoryAndDoesntSkipFailedFilesInParallel)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

            string sampleDirToOptimize = FileProcessingTestsHelpers.PrepareTestOptimizeDir("SampleDirToOptimizeBrokenJpg", fileNameFileProcessedStateRememberer, fileNameDirProcessedStateRememberer, testName);

            //Optimize first time                
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }

            //Optimize second time
            {
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(2, results.Count);
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Success));
                Assert.Equal(1, results.Count(t => t.OptimizationResult == OptimizationResult.Failed));
            }
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesReadonlyAndBlockedFilesInDirectory()
        {
            var config = ConfigCreator.CreateTestConfig(true);

            var testName = $"{nameof(FileProcessorDirTests)}_{nameof(CorrectlyOptimizesReadonlyAndBlockedFilesInDirectory)}";
            var fileNameFileProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}.txt");
            var fileNameDirProcessedStateRememberer = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, $"{testName}-dir.txt");

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
                var rememberer = new FileProcessedStateRememberer(true, fileNameFileProcessedStateRememberer);
                var dirRememberer = new DirProcessedStateRememberer(false, fileNameDirProcessedStateRememberer);
                var fp = new DeveImageOptimizerProcessor(config, null, rememberer, dirRememberer);

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
