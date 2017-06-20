using DeveImageOptimizer.Helpers;
using ImageSharp;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class ReproduceBugImageSharp
    {
        [Fact]
        public void ReproducesBug()
        {
            var startupAssembly = typeof(ReproduceBugImageSharp).GetTypeInfo().Assembly;
            var cb = startupAssembly.CodeBase;

            UriBuilder uri = new UriBuilder(cb);
            string path = Uri.UnescapeDataString(uri.Path);
            var assemblyDir = Path.GetDirectoryName(path);

            var image1path = Path.Combine(assemblyDir, "TestImages", "Image1.JPG");

            var img = Image.Load(image1path);

            int width = img.Width;
            int height = img.Height;

            int x = 5990;
            int y = 3992;

            var thePixel = img.Pixels[y * width + x];

            if (thePixel.R == 36 && thePixel.G == 15 && thePixel.B == 10)
            {
                throw new Exception("Image 1 probably");
            }

            if (thePixel.R == 38 && thePixel.G == 14 && thePixel.B == 12)
            {
                Console.WriteLine("Goed :)");
            }
            else
            {
                throw new Exception("KAPOT");
            }
        }
    }
}
