using DeveImageOptimizer.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.TestHelpers
{
    public static class ImageComparerAndWriteOutputDifferences
    {
        public static async Task<int> CompareTheseImagesAndWriteResultToOutputAsync(string image1, string image2, string investigationname, string outputImageName)
        {
            var outputDir = Path.Combine(FolderHelperMethods.Internal_TempForTestDirectory.Value, investigationname);
            Directory.CreateDirectory(outputDir);

            var outputImage = Path.Combine(outputDir, outputImageName + ".png");
            var outputImage2 = Path.Combine(outputDir, outputImageName + "2.png");

            int pixelsWrong = 0;

            using (var img1 = Image.Load<Rgba32>(image1))
            using (var img2 = Image.Load<Rgba32>(image2))
            {
                Assert.Equal(img1.Width, img2.Width);
                Assert.Equal(img1.Height, img2.Height);

                using (var outputImg = new Image<Rgba32>(img1.Width, img1.Height))
                using (var outputImg2 = new Image<Rgba32>(img1.Width, img1.Height))
                {
                    for (int y = 0; y < img1.Height; y++)
                    {
                        for (int x = 0; x < img1.Width; x++)
                        {
                            var pixel1 = img1[x, y];
                            var pixel2 = img2[x, y];

                            if (pixel1 != pixel2 && (pixel1.A != 0 || pixel2.A != 0))
                            {
                                outputImg[x, y] = new Rgba32(255, 0, 0);
                                outputImg2[x, y] = new Rgba32(255, 0, 0);
                                pixelsWrong++;
                            }
                            else
                            {
                                outputImg[x, y] = pixel1;
                                outputImg2[x, y] = new Rgba32(255, 255, 255);
                            }
                        }
                    }

                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        outputImg.SaveAsPng(fs);
                    }

                    using (var fs = new FileStream(outputImage2, FileMode.Create))
                    {
                        outputImg2.SaveAsPng(fs);
                    }
                }
            }



            var imageIsEqual = await ImageComparer.AreImagesEqualAsync(image1, image2);
            //If this fails something is wrong in either the ImageEqual method or this test method
            Assert.Equal(imageIsEqual, pixelsWrong == 0);

            Console.WriteLine($"Image Comparison: Compared: {image1} to {image2} Pixels wrong: {pixelsWrong}");
            return pixelsWrong;
        }
    }
}

