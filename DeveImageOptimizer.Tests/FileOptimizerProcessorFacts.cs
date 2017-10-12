using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
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
        public async Task CorrectlyOptimizedPexelsPhoto()
        {
            await OptimizeFileTest("pexels-photo.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizedVimPicture()
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
        public async Task CorrectlyOptimizedDevtoolsSidePaneImage()
        {
            await OptimizeFileTest("devtools-side-pane_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizedImageSharpImage1()
        {
            await OptimizeFileTest("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizedImageSharpImage2()
        {
            await OptimizeFileTest("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizedImageSharpImage3()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizedImageSharpImage4()
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
                var worked = await fop.OptimizeFile(image1temppath);

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
