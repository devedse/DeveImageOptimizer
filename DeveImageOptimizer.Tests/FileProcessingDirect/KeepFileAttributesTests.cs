using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessingDirect
{
    public class KeepFileAttributesTests
    {
        [SkippableFact, Trait(TraitNames.CallsFileOptimizer, Trait.True)]
        public async Task DoesNotKeepAttributesWhenKeepFIleAttributesIsFalse()
        {
            var fileName = "GoProBison.JPG";

            var config = ConfigCreator.CreateTestConfig();
            config.KeepFileAttributes = false;

            var fop = new FileOptimizerProcessor(config);
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            var originalCreationTime = File.GetCreationTimeUtc(image1temppath);
            var originalModifiedTime = File.GetLastWriteTimeUtc(image1temppath);

            try
            {
                var fileToOptimize = new OptimizableFile(image1temppath, null, new FileInfo(image1temppath).Length);

                await fop.OptimizeFile(fileToOptimize);

                Assert.Equal(OptimizationResult.Success, fileToOptimize.OptimizationResult);

                var fileOptimized = new FileInfo(image1temppath);
                var fileUnoptimized = new FileInfo(image1path);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimized.Length <= fileUnoptimized.Length);

                var optimizedCreationTime = File.GetCreationTimeUtc(image1temppath);
                var optimizedModifiedTime = File.GetLastWriteTimeUtc(image1temppath);

                Assert.Equal(originalCreationTime, optimizedCreationTime);
                Assert.True(originalModifiedTime < optimizedModifiedTime);
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }

        [SkippableFact, Trait(TraitNames.CallsFileOptimizer, Trait.True)]
        public async Task KeepsAttributesWhenConfigured()
        {
            var fileName = "GoProBison.JPG";

            var config = ConfigCreator.CreateTestConfig();
            config.KeepFileAttributes = true;

            var fop = new FileOptimizerProcessor(config);
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            var originalCreationTime = File.GetCreationTimeUtc(image1temppath);
            var originalModifiedTime = File.GetLastWriteTimeUtc(image1temppath);

            try
            {
                var fileToOptimize = new OptimizableFile(image1temppath, null, new FileInfo(image1temppath).Length);

                await fop.OptimizeFile(fileToOptimize);

                Assert.Equal(OptimizationResult.Success, fileToOptimize.OptimizationResult);

                var fileOptimized = new FileInfo(image1temppath);
                var fileUnoptimized = new FileInfo(image1path);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimized.Length <= fileUnoptimized.Length);

                var optimizedCreationTime = File.GetCreationTimeUtc(image1temppath);
                var optimizedModifiedTime = File.GetLastWriteTimeUtc(image1temppath);

                Assert.Equal(originalCreationTime, optimizedCreationTime);
                Assert.Equal(originalModifiedTime, optimizedModifiedTime);
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }
    }
}
