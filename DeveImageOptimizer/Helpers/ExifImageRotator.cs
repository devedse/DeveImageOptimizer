using ExifLibrary;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ExifImageRotator
    {
        public static async Task<Orientation?> UnrotateImageAsync(string path)
        {
            var file = await JPEGFile.FromFileAsync(path);

            ExifProperty? orientationExif = file.Properties.FirstOrDefault(t => t.Tag == ExifTag.Orientation);

            if (orientationExif != null)
            {
                var retval = (Orientation)orientationExif.Value;
                if (retval != Orientation.Normal)
                {
                    orientationExif.Value = Orientation.Normal;

                    await file.SaveAsync(path);

                    return retval;
                }
            }
            return null;
        }

        public static async Task RerotateImageAsync(string path, Orientation? newOrientation)
        {
            if (newOrientation != null)
            {
                var file = await ImageFile.FromFileAsync(path);

                ExifProperty? orientationExif = file.Properties.FirstOrDefault(t => t.Tag == ExifTag.Orientation);

                if (orientationExif != null)
                {
                    orientationExif.Value = newOrientation;
                }
                else
                {
                    throw new InvalidOperationException("MetaData is not kept equal");
                }

                await file.SaveAsync(path);
            }
        }
    }
}
