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
    public class FileOptimizerProcessorFacts
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesLandscapeImage()
        {
            await OptimizeFileTest("Image1A.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesStandingRotatedImage()
        {
            await OptimizeFileTest("Image2A.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesPexelsPhoto()
        {
            await OptimizeFileTest("pexels-photo.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesVimPicture()
        {
            await OptimizeFileTest("vim16x16_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGifImage()
        {
            await OptimizeFileTest("devtools-full_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesVersioningImage()
        {
            await OptimizeFileTest("versioning-1_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesSnakeImage()
        {
            await OptimizeFileTest("snake.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesCraDynamicImportImage()
        {
            await OptimizeFileTest("cra-dynamic-import_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesDevtoolsSidePaneImage()
        {
            await OptimizeFileTest("devtools-side-pane_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage1()
        {
            await OptimizeFileTest("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage2()
        {
            await OptimizeFileTest("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage3()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage4()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGifSourceImage()
        {
            //This image doesn't optimize correctly. This is a bug in FileOptimizerFull
            await OptimizeFileTest("Source.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesIconImage()
        {
            await OptimizeFileTest("icon_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGoProImage()
        {
            await OptimizeFileTest("GoProBison.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesSmileFaceBmpImage()
        {
            await OptimizeFileTest("SmileFaceBmp.bmp");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesPezImageWithSpaceInName()
        {
            await OptimizeFileTest("pez image with space.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesAfterJpgImage()
        {
            await OptimizeFileTest("After.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesRuthImage()
        {
            await OptimizeFileTest("baberuth_1.png");
        }

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

        private async Task OptimizeFileTest(string fileName)
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            try
            {
                var worked = await fop.OptimizeFile(image1temppath, null);

                Assert.True(worked.Successful);

                var fileOptimized = new FileInfo(image1temppath);
                var fileUnoptimized = new FileInfo(image1path);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimized.Length < fileUnoptimized.Length);
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }
    }
}
