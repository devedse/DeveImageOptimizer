using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.State
{
    public class WholeDirectoryOptimizationTests
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesCompleteDirectoryAndDoesntOptimizeSecondTime()
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var sourceSampleDirToOptimize = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "SampleDirToOptimize");
            var sampleDirToOptimize = Path.Combine(tempfortestdir, "SampleDirToOptimize");

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

            //Optimize first time                
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
                var rememberer = new FileProcessedStateRememberer(false);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.True(result.Successful);
                    Assert.False(result.SkippedBecausePreviouslyOptimized);
                    Assert.True(result.OriginalSize > result.OptimizedSize);
                }
            }

            //Optimize second time
            {
                var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
                var rememberer = new FileProcessedStateRememberer(false);
                var fp = new FileProcessor(fop, null, rememberer);

                var results = (await fp.ProcessDirectory(sampleDirToOptimize)).ToList();

                Assert.Equal(4, results.Count);
                foreach (var result in results)
                {
                    Assert.True(result.Successful);
                    Assert.True(result.SkippedBecausePreviouslyOptimized);
                    Assert.True(result.OriginalSize == result.OptimizedSize);
                }
            }
        }
    }
}
