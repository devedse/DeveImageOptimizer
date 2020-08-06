using System.Linq;

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
    }
}
