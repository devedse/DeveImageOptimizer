using ImageSharp;
using System;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class ReproduceBugImageSharp
    {
        [Fact]
        public void ReproducesBug()
        {
            var img = Image.Load(@"DeveImageOptimizer.Tests\TestImages\Image1.JPG");

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
