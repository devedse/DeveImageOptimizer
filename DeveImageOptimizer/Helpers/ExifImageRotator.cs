using ExifLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ExifImageRotator
    {
        public async static Task<Orientation> UnrotateImageAsync(string path)
        {
            return await Task.Run(() =>
            {
                return UnrotateImage(path);
            });
        }

        private static Orientation UnrotateImage(string path)
        {
            var file = ExifFile.Read(path);

            var orientationExif = file.Properties[ExifTag.Orientation];
            var retval = (Orientation)orientationExif.Value;
            orientationExif.Value = Orientation.Normal;

            file.Save(path);

            return retval;
        }

        public async static Task RerotateImageAsync(string path, Orientation newOrientation)
        {
            await Task.Run(() =>
            {
                RerotateImage(path, newOrientation);
            });
        }

        private static void RerotateImage(string path, Orientation newOrientation)
        {
            var file = ExifFile.Read(path);

            var orientationExif = file.Properties[ExifTag.Orientation];
            orientationExif.Value = newOrientation;

            file.Save(path);
        }
    }
}
