using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
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
            var imagePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            try
            {
                using (var img = Image.Load(imagePath))
                {
                    using (var fs = new FileStream(imageTempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
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
            var imagePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            try
            {
                using (var img = Image.Load(imagePath))
                {
                    using (var fs = new FileStream(imageTempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
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
            var imagePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            using (var image = Image.Load<Rgba32>(imagePath))
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
            var imagePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var imageTempPath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName, "png"));

            Directory.CreateDirectory(tempfortestdir);

            using (var image = Image.Load<Rgba32>(imagePath))
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
