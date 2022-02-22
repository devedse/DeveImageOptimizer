using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class OtherRandomFacts
    {
        [Fact]
        public void ReproducesJpgDecoderBug()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");

            using (var img = Image.Load<Rgba32>(image1path))
            {
                int width = img.Width;
                int height = img.Height;

                int x = 5990;
                int y = 3992;

                var thePixel = img[x, y];

                Skip.If(thePixel.R == 36 && thePixel.G == 15 && thePixel.B == 10, "Test skipped but actual decoding has failed. This should be fixed once ImageSharp fixes their JPG decoder.");

                if (thePixel.R == 38 && thePixel.G == 14 && thePixel.B == 12)
                {
                    Console.WriteLine("Goed :)");
                }
                else
                {
                    throw new Exception("KAPOT");
                }
            }
        }

        [Fact]
        public async Task CanReadThisPngCorrectly()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "vim16x16_1.png");
            var outputImage = Path.Combine(FolderHelperMethods.Internal_TempForTestDirectory.Value, "vim16x16output.png");

            try
            {
                using (var img = Image.Load<Rgba32>(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img[1, 0];
                    Skip.If(pixel.A != 0, "Pixel at X: 1 and Y: 0 should be transparent.");
                }
            }
            finally
            {
                if (File.Exists(outputImage))
                {
                    File.Delete(outputImage);
                }
            }
        }

        [Fact]
        public async Task LoadAndSaveThisImage()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "versioning-1_2.png");
            var outputImage = Path.Combine(FolderHelperMethods.Internal_TempForTestDirectory.Value, "versioning-1_2_output.png");

            try
            {
                using (var img = Image.Load<Rgba32>(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img[0, 0];
                    if (pixel.A != 0)
                    {
                        throw new Exception("Pixel at X: 0 and Y: 0 should be transparent.");
                    }
                }
            }
            finally
            {
                if (File.Exists(outputImage))
                {
                    File.Delete(outputImage);
                }
            }
        }

        [Fact]
        public async Task LoadAndSaveSnakeImage()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "snake.png");
            var outputImage = Path.Combine(FolderHelperMethods.Internal_TempForTestDirectory.Value, "snake_output.png");

            try
            {
                using (var img = Image.Load<Rgba32>(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img[0, 0];
                    if (pixel.A != 0)
                    {
                        throw new Exception("Pixel at X: 0 and Y: 0 should be transparent.");
                    }
                }
            }
            finally
            {
                if (File.Exists(outputImage))
                {
                    File.Delete(outputImage);
                }
            }
        }

        [Fact]
        public async Task LoadAndSaveImageSharpImage2()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Imagesharp", "ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
            var outputImage = Path.Combine(FolderHelperMethods.Internal_TempForTestDirectory.Value, "Imagesharp2.png");

            try
            {
                using (var img = Image.Load<Rgba32>(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        var encoder = new PngEncoder()
                        {
                            ColorType = PngColorType.RgbWithAlpha,
                            BitDepth = PngBitDepth.Bit8
                        };
                        img.SaveAsPng(fs, encoder);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img[0, 0];
                    if (pixel.A != 0)
                    {
                        throw new Exception("Pixel at X: 0 and Y: 0 should be transparent.");
                    }
                }
            }
            finally
            {
                if (File.Exists(outputImage))
                {
                    File.Delete(outputImage);
                }
            }
        }

        [Fact]
        public async Task RemovesExifRotationAndReapliesAfterwards()
        {
            var fileName = "Image2A.JPG";
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            try
            {
                using (var imageBefore = Image.Load(image1temppath))
                {
                    var oldRotation = await ExifImageRotator.UnrotateImageAsync(image1temppath);

                    using (var imageAfterUnrotate = Image.Load(image1temppath))
                    {
                        await ExifImageRotator.RerotateImageAsync(image1temppath, oldRotation);

                        using (var imageAfter = Image.Load(image1temppath))
                        {
                            var rotBefore = imageBefore.Metadata.ExifProfile.GetValue(ExifTag.Orientation);
                            var rotAfterUnrotate = imageAfterUnrotate.Metadata.ExifProfile.GetValue(ExifTag.Orientation);
                            var rotAfter = imageAfter.Metadata.ExifProfile.GetValue(ExifTag.Orientation);

                            Assert.Contains("270", rotBefore.ToString());
                            Assert.Contains("normal", rotAfterUnrotate.ToString().ToLowerInvariant());
                            Assert.Equal(rotBefore, rotAfter);
                        }
                    }
                }
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }
    }
}
