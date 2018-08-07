using ExifLibrary;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ExifImageRotator
    {
        public async static Task<Orientation?> UnrotateImageAsync(string path)
        {
            return await Task.Run(() =>
            {
                return UnrotateImage(path);
            });
        }

        private static Orientation? UnrotateImage(string path)
        {
            var file = JPEGFile.FromFile(path);

            ExifProperty orientationExif = file.Properties.FirstOrDefault(t => t.Tag == ExifTag.Orientation);

            if (orientationExif != null)
            {
                var retval = (Orientation)orientationExif.Value;
                orientationExif.Value = Orientation.Normal;

                file.Save(path);

                return retval;
            }
            else
            {
                return null;
            }
        }

        public async static Task RerotateImageAsync(string path, Orientation? newOrientation)
        {
            await Task.Run(() =>
            {
                RerotateImage(path, newOrientation);
            });
        }

        private static void RerotateImage(string path, Orientation? newOrientation)
        {
            if (newOrientation != null)
            {
                var file = ImageFile.FromFile(path);

                ExifProperty orientationExif = file.Properties.FirstOrDefault(t => t.Tag == ExifTag.Orientation);

                if (orientationExif != null)
                {
                    orientationExif.Value = newOrientation;
                }
                else
                {
                    throw new InvalidOperationException("MetaData is not kept equal");
                }

                file.Save(path);
            }
        }
    }
}
