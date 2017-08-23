using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using ImageSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class FileOptimizerProcessorFacts
    {
        [SkippableFact]
        public async void CorrectlyOptimizesLandscapeImage()
        {
            await OptimizeFileTest("Image1A.JPG");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesStandingRotatedImage()
        {
            await OptimizeFileTest("Image2A.JPG");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedPexelsPhoto()
        {
            await OptimizeFileTest("pexels-photo.jpg");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedVimPicture()
        {
            await OptimizeFileTest("vim16x16_1.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesGifImage()
        {
            await OptimizeFileTest("devtools-full_1.gif");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesVersioningImage()
        {
            await OptimizeFileTest("versioning-1_1.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesSnakeImage()
        {
            await OptimizeFileTest("snake.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesCraDynamicImportImage()
        {
            await OptimizeFileTest("cra-dynamic-import_1.gif");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedDevtoolsSidePaneImage()
        {
            await OptimizeFileTest("devtools-side-pane_1.gif");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedImageSharpImage1()
        {
            await OptimizeFileTest("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedImageSharpImage2()
        {
            await OptimizeFileTest("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedImageSharpImage3()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizedImageSharpImage4()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async void CorrectlyOptimizesIconImage()
        {
            await OptimizeFileTest("icon_1.png");
        }       

        private async Task OptimizeFileTest(string fileName)
        {
            var fileOptimizerPath = @"C:\Program Files\FileOptimizer\FileOptimizer64.exe";

            Skip.IfNot(File.Exists(fileOptimizerPath), $"FileOptimizerFull exe file can't be found. Expected location: {fileOptimizerPath}");

            var fop = new FileOptimizerProcessor(fileOptimizerPath, FolderHelperMethods.TempDirectoryForTests.Value);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "TempForTest");
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
                Directory.Delete(tempfortestdir, true);
            }
        }
    }
}
