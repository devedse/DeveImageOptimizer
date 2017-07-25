using DeveImageOptimizer.ImageConversion;
using ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ImageComparer2
    {
        public static async Task<bool> AreImagesEqualAsync(string image1Path, string image2Path)
        {
            return await Task.Run(() =>
            {
                return AreImagesEqual(image1Path, image2Path);
            });
        }

        private static bool AreImagesEqual(string image1Path, string image2Path)
        {
            var w = Stopwatch.StartNew();

            var extension1 = Path.GetExtension(image1Path);
            var extension2 = Path.GetExtension(image2Path);

            var tempFiles = new List<string>();

            try
            {
                if (string.Equals(extension1, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension1, ".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    image1Path = ImageConverter.ConvertJpgToPng(image1Path);
                    tempFiles.Add(image1Path);
                }

                if (string.Equals(extension2, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension2, ".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    image2Path = ImageConverter.ConvertJpgToPng(image2Path);
                    tempFiles.Add(image2Path);
                }


                var image1 = Image.Load(image1Path);
                var image2 = Image.Load(image2Path);


                if (image1.Width != image2.Width || image1.Height != image2.Height)
                {
                    return false;
                }

                int width = image1.Width;
                int height = image1.Height;

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
                                Console.WriteLine($"Failed, elapsed time: {w.Elapsed}");
                                return false;
                            }
                        }
                    }
                }

                Console.WriteLine($"Elapsed time: {w.Elapsed}");

                return true;
            }
            finally
            {
                foreach(var tempFile in tempFiles)
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
            }
        }
    }
}
