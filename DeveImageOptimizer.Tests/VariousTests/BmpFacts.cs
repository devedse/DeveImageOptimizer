using DeveImageOptimizer.Helpers;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class BmpFacts
    {
        [Fact]
        public async Task BmpFileShouldBeEqualToBmpConvertedAsPng()
        {
            var fileName = "SmileFaceBmp.bmp";
            var imagePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            try
            {
                using (var image = Image.Load(imagePath))
                {
                    using (var fs = new FileStream(imageTempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        image.SaveAsPng(fs);
                    }
                }

                using (var image2 = Image.Load(imageTempPath))
                {
                    var result = await ImageComparer.AreImagesEqualAsync(imagePath, imageTempPath);
                    Assert.True(result);
                }
            }
            finally
            {
                File.Delete(imageTempPath);
            }
        }

        [Fact]
        public async Task Bmp2FileShouldBeEqualToBmpConvertedAsPng()
        {
            var fileName = "SmileFaceBmp2.bmp";
            var imagePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            try
            {
                using (var image = Image.Load(imagePath))
                {
                    using (var fs = new FileStream(imageTempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        image.SaveAsPng(fs);
                    }
                }

                using (var image2 = Image.Load(imageTempPath))
                {
                    var result = await ImageComparer.AreImagesEqualAsync(imagePath, imageTempPath);
                    Assert.True(result);
                }
            }
            finally
            {
                File.Delete(imageTempPath);
            }
        }

        [Fact]
        public void BmpShouldNotContainAlpha()
        {
            var fileName = "SmileFaceBmp.bmp";
            var imagePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            using (var image = Image.Load(imagePath))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Assert.Equal(255, image[x, y].A);
                    }
                }
            }
        }

        [Fact]
        public void Bmp2ShouldNotContainAlpha()
        {
            var fileName = "SmileFaceBmp2.bmp";
            var imagePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            using (var image = Image.Load(imagePath))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Assert.Equal(255, image[x, y].A);
                    }
                }
            }
        }
    }
}
