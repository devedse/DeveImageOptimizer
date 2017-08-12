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

        [Fact]
        public async void RemovesExifRotationAndReapliesAfterwards()
        {
            var fileName = "Image2A.JPG";
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "TempForTest");
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            try
            {
                var imageBefore = Image.Load(image1temppath);

                var oldRotation = await ExifImageRotator.UnrotateImageAsync(image1temppath);

                var imageAfterUnrotate = Image.Load(image1temppath);

                await ExifImageRotator.RerotateImageAsync(image1temppath, oldRotation);

                var imageAfter = Image.Load(image1temppath);

                var rotBefore = imageBefore.MetaData.ExifProfile.GetValue(ExifTag.Orientation);
                var rotAfterUnrotate = imageAfterUnrotate.MetaData.ExifProfile.GetValue(ExifTag.Orientation);
                var rotAfter = imageAfter.MetaData.ExifProfile.GetValue(ExifTag.Orientation);

                Assert.True(rotBefore.ToString().Contains("270"));
                Assert.True(rotAfterUnrotate.ToString().ToLowerInvariant().Contains("normal"));
                Assert.Equal(rotBefore, rotAfter);
            }
            finally
            {
                File.Delete(image1temppath);
                Directory.Delete(tempfortestdir);
            }
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
