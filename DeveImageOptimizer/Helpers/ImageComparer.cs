using System.Drawing;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ImageComparer
    {
        public static async Task<bool> AreImagesEqualAsync(string image1Path, string image2Path)
        {
            //on bitmap asynchronously
            return await Task.Run(() =>
            {
                return AreImagesEqual(image1Path, image2Path);
            });
        }

        public static bool AreImagesEqual(string image1Path, string image2Path)
        {
            using (var image1 = new Bitmap(image1Path))
            {
                using (var image2 = new Bitmap(image2Path))
                {
                    if (image1.Size != image2.Size)
                        return false;

                    for (int y = 0; y < image1.Height; y++)
                    {
                        for (int x = 0; x < image1.Width; x++)
                        {
                            var pixel1 = image1.GetPixel(x, y);
                            var pixel2 = image2.GetPixel(x, y);

                            if (pixel1 != pixel2)
                                return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
