using DeveImageOptimizer.FileProcessing;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DeveImageOptimizer
{
    public static class ConstantsFileExtensions
    {
        public static string[] JPGExtensions { get; } = new string[] { ".JFI", ".JIF", ".JFIF", ".JNG", ".JPE", ".JPG", ".JPEG", ".THM" }; //".MPO", ".JPS"
        public static string[] PNGExtensions { get; } = new string[] { ".APNG", ".PNG" }; //".ICO", ".PNS"
        public static string[] GIFExtensions { get; } = new string[] { ".GIF" };
        public static string[] BMPExtensions { get; } = new string[] { ".BMP" }; //".DIB"

        public static string[] AllValidExtensions { get; }
        static ConstantsFileExtensions()
        {
            AllValidExtensions = JPGExtensions.Concat(PNGExtensions).Concat(GIFExtensions).Concat(BMPExtensions).ToArray();
        }

        public static bool ShouldSkipBasedOnConfiguration(DeveImageOptimizerConfiguration configuration, string fileExtension)
        {
            var upperExt = fileExtension.ToUpper();

            //Check skipping
            switch (upperExt)
            {
                case var e when !configuration.OptimizeJpg && JPGExtensions.Contains(e.ToUpperInvariant()):
                    {
                        return true;
                    }
                case var e when !configuration.OptimizePng && PNGExtensions.Contains(e.ToUpperInvariant()):
                    {
                        return true;
                    }
                case var e when !configuration.OptimizeGif && GIFExtensions.Contains(e.ToUpperInvariant()):
                    {
                        return true;
                    }
                case var e when !configuration.OptimizeBmp && BMPExtensions.Contains(e.ToUpperInvariant()):
                    {
                        return true;
                    }
            }

            return false;
        }
    }
}
