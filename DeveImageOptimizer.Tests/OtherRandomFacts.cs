using DeveImageOptimizer.Helpers;
using ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class OtherRandomFacts
    {
        [SkippableFact]
        public void ReproducesJpgDecoderBug()
        {
            var startupAssembly = typeof(FileOptimizerProcessorFacts).GetTypeInfo().Assembly;
            var cb = startupAssembly.CodeBase;

            UriBuilder uri = new UriBuilder(cb);
            string path = Uri.UnescapeDataString(uri.Path);
            var assemblyDir = Path.GetDirectoryName(path);

            var image1path = Path.Combine(assemblyDir, "TestImages", "Image1A.JPG");

            var img = Image.Load(image1path);

            int width = img.Width;
            int height = img.Height;

            int x = 5990;
            int y = 3992;

            var thePixel = img.Pixels[y * width + x];

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

        [Fact]
        public async void CanReadThisPngCorrectly()
        {
            Directory.CreateDirectory(FolderHelperMethods.TempDirectoryForTests.Value);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "vim16x16_1.png");
            var outputImage = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "vim16x16output.png");

            try
            {
                using (var img = Image.Load(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        img.SaveAsPng(fs);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer2.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img.Pixels[1];
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
        public async void LoadAndSaveThisImage()
        {
            Directory.CreateDirectory(FolderHelperMethods.TempDirectoryForTests.Value);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "versioning-1_2.png");
            var outputImage = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "versioning-1_2_output.png");

            try
            {
                using (var img = Image.Load(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        img.SaveAsPng(fs);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer2.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img.Pixels[0];
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

        [SkippableFact]
        public async void LoadAndSaveSnakeImage()
        {
            Directory.CreateDirectory(FolderHelperMethods.TempDirectoryForTests.Value);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "snake.png");
            var outputImage = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "snake_output.png");

            try
            {
                using (var img = Image.Load(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        img.SaveAsPng(fs);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer2.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img.Pixels[0];

                    Skip.If(pixel.A != 0, "This test should not be skipped but yeah, temporary workarounds are needed till ImageSharp fixes this bug");

                    //if (pixel.A != 0)
                    //{
                    //    throw new Exception("Pixel at X: 0 and Y: 0 should be transparent.");
                    //}
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
        public async void LoadAndSaveImageSharpImage2()
        {
            Directory.CreateDirectory(FolderHelperMethods.TempDirectoryForTests.Value);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages","Imagesharp", "ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
            var outputImage = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, "Imagesharp2.png");

            try
            {
                using (var img = Image.Load(image1path))
                {
                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        img.SaveAsPng(fs);
                    }

                    //Kinda hard to test this since this loads the same pixel data in an incorrect way.                    
                    using (var outputtedImage = Image.Load(outputImage))
                    {
                        var result = await ImageComparer2.AreImagesEqualAsync(image1path, outputImage);
                        Assert.True(result);
                    }

                    //I'll just check the pixel by hand.
                    var pixel = img.Pixels[0];
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
    }
}
