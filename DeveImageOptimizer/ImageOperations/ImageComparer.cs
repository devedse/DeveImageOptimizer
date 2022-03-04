using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOperations
{
    public static class ImageComparer
    {
        public static Task<bool> AreImagesEqualAsync(string image1Path, string image2Path)
        {
            return Task.Run(async () =>
            {
                var imagePixelsEqual = ImagePixelComparer.AreImagePixelsEqual(image1Path, image2Path);

                //Commented out imageExifEqual because it doesn't work correctly yet
                //var imageExifEqual = await ImageExifComparer.AreImageExifDatasEqual(image1Path, image2Path);
                var imageExifEqual = true;

                return imagePixelsEqual && imageExifEqual;
            });
        }
    }
}
