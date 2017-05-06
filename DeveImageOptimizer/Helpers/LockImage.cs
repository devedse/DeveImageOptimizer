using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public class LockBitmap
    {
        private Bitmap source = null;
        private IntPtr Iptr = IntPtr.Zero;
        private BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;

            // Get width and height of bitmap
            Width = source.Width;
            Height = source.Height;
        }

        public void LockBits()
        {
            // get total locked pixels count
            int pixelCount = Width * Height;

            // Create rectangle to lock
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // get source bitmap pixel format size
            Depth = Image.GetPixelFormatSize(source.PixelFormat);

            // Check if bpp (Bits Per Pixel) is 8, 24, or 32

            if (Depth == 4)
            {
                //God knows what to do here, so we just fallback to good old getpixel method
                return;
            }
            if (Depth != 8 && Depth != 24 && Depth != 32 && Depth != 1)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }

            // Lock bitmap and return bitmap data
            bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat);

            // create byte array to copy pixel values
            // This was the old shitty code from the internet (which doesn't work with Depth < 8):
            // int step = Depth / 8;
            // This is correct:
            Pixels = new byte[bitmapData.Stride * Height];
            Iptr = bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
        }

        public void UnlockBits()
        {
            // Copy data from byte array to pointer
            Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

            // Unlock bitmap data
            source.UnlockBits(bitmapData);
        }

        public Color GetPixel(int x, int y)
        {
            if (Depth == 4)
            {
                //Don't really know how to do this
                return source.GetPixel(x, y);
            }

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                return Color.FromArgb(a, r, g, b);
            }
            else if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                return Color.FromArgb(r, g, b);
            }
            else if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                if (source.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    byte c = Pixels[i];
                    return source.Palette.Entries[c];
                }
                else
                {
                    byte c = Pixels[i];
                    return Color.FromArgb(c, c, c);
                }
            }
            else if (Depth == 1)
            {
                byte c = Pixels[i];
                if (source.PixelFormat == PixelFormat.Format1bppIndexed)
                {
                    var bitNumber = i % 8;
                    var bit = (c & (1 << bitNumber - 1)) != 0;
                    if (bit)
                    {
                        return Color.Black;
                    }
                    else
                    {
                        return Color.White;
                    }
                }
                else
                {
                    throw new FormatException("Don't know but it's broken");
                }
            }            
            else
            {
                throw new FormatException($"An image with depth {Depth} is not supported");
            }
        }
    }
}
