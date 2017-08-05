using DeveImageOptimizer.ImageConversion;
using ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ImageComparer2
    {
        public static async Task<bool> AreImagesEqualAsync(string image1Path, string image2Path, bool useImageSharpBugWorkaround = true)
        {
            return await Task.Run(() =>
            {
                return AreImagesEqual(image1Path, image2Path, useImageSharpBugWorkaround);
            });
        }

        private static string GetRotatedAmount(Image<Rgba32> source)
        {

            var blah = source?.MetaData?.ExifProfile?.GetValue(ExifTag.Orientation);
            if (blah == null)
            {
                return "0";
            }

            var blahString = blah.ToString();

            var digits = blahString.Where(t => char.IsDigit(t)).ToArray();
            if (!digits.Any())
            {
                return "0";
            }
            return new string(digits);
        }

        private static Image<Rgba32> LoadImageHelper(string imagePath, List<string> tempFiles, bool useImageSharpBugWorkaround)
        {
            if (useImageSharpBugWorkaround && FileTypeHelper.IsJpgFile(imagePath))
            {
                var imageAsPngPath = ImageConverter.ConvertJpgToPngWithAutoRotate(imagePath);
                tempFiles.Add(imageAsPngPath);
                return Image.Load(imageAsPngPath);
            }
            else
            {
                return Image.Load(imagePath);
            }
        }

        private static bool AreImagesEqual(string image1Path, string image2Path, bool useImageSharpBugWorkaround)
        {
            if (!File.Exists(image1Path)) throw new FileNotFoundException("Could not find Image1", image1Path);
            if (!File.Exists(image2Path)) throw new FileNotFoundException("Could not find Image2", image2Path);

            var w = Stopwatch.StartNew();

            var tempFiles = new List<string>();

            try
            {
                using (var image1 = LoadImageHelper(image1Path, tempFiles, useImageSharpBugWorkaround))
                using (var image2 = LoadImageHelper(image2Path, tempFiles, useImageSharpBugWorkaround))
                {
                    image1.AutoOrient();
                    image2.AutoOrient();

                    if (image1.Width != image2.Width || image1.Height != image2.Height)
                    {
                        return false;
                    }

                    int width = image1.Width;
                    int height = image1.Height;

                    long pixelsWrong = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var pixel1 = image1.Pixels[y * width + x];
                            var pixel2 = image2.Pixels[y * width + x];

                            if (pixel1 != pixel2)
                            {
                                if (pixel1.A == 0 && pixel2.A == 0)
                                {
                                    //Optimization that happens to better be able to compress png's sometimes
                                    //Fully transparent pixel so the pixel is still the same no matter what the RGB values
                                }
                                else
                                {
                                    //Console.WriteLine($"Failed, elapsed time: {w.Elapsed}");
                                    //return false;
                                    pixelsWrong++;
                                }
                            }
                        }
                    }
                    
                    Console.WriteLine($"Image comparison done in: {w.Elapsed}. Wrong pixels: {pixelsWrong}");
                    if (pixelsWrong > 0)
                    {
                        return false;
                    }

                    return true;
                }
            }
            finally
            {
                foreach (var tempFile in tempFiles)
                {
                    FileHelperMethods.SafeDeleteTempFile(tempFile);
                }
            }
        }
    }
}
