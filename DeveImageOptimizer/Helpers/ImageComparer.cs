using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ImageComparer
    {
        public static async Task<bool> AreImagesEqualAsync(string image1Path, string image2Path)
        {
            return await Task.Run(() =>
            {
                return AreImagesEqual(image1Path, image2Path);
            });
        }

        public static async Task<bool> AreImagesEqualAsync2(string image1Path, string image2Path)
        {
            return await Task.Run(() =>
            {
                return AreImagesEqual2(image1Path, image2Path);
            });
        }

        public static bool AreImagesEqual(string image1Path, string image2Path)
        {
            var w = Stopwatch.StartNew();

            using (var image1 = new Bitmap(image1Path))
            {
                using (var image2 = new Bitmap(image2Path))
                {
                    if (image1.Size != image2.Size)
                    {
                        return false;
                    }

                    for (int y = 0; y < image1.Height; y++)
                    {
                        for (int x = 0; x < image1.Width; x++)
                        {
                            var pixel1 = image1.GetPixel(x, y);
                            var pixel2 = image2.GetPixel(x, y);

                            if (pixel1 != pixel2)
                            {
                                if (pixel1.A == 0 && pixel2.A == 0)
                                {
                                    //Optimization that happens to better be able to compress png's sometimes
                                }
                                return false;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Elapsed for Equal1: " + w.Elapsed.TotalSeconds);

            return true;
        }

        public static bool AreImagesEqual2(string image1Path, string image2Path)
        {
            var w = Stopwatch.StartNew();

            using (var image1 = new Bitmap(image1Path))
            {
                using (var image2 = new Bitmap(image2Path))
                {
                    if (image1.Size != image2.Size)
                    {
                        return false;
                    }

                    var lockImage1 = new LockBitmap(image1);
                    var lockImage2 = new LockBitmap(image2);

                    lockImage1.LockBits();
                    lockImage2.LockBits();

                    for (int y = 0; y < lockImage1.Height; y++)
                    {
                        for (int x = 0; x < lockImage2.Width; x++)
                        {
                            var pixel1 = lockImage1.GetPixel(x, y);
                            var pixel2 = lockImage2.GetPixel(x, y);

                            if (pixel1 != pixel2)
                            {
                                if (pixel1.A == 0 && pixel2.A == 0)
                                {
                                    //Optimization that happens to better be able to compress png's sometimes
                                }
                                return false;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Elapsed for Equal2: " + w.Elapsed.TotalSeconds);

            return true;
        }
    }
}
