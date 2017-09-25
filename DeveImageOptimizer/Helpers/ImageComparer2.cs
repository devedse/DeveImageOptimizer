using DeveImageOptimizer.ImageConversion;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class ImageComparer2
    {
        public static Task<bool> AreImagesEqualAsync(string image1Path, string image2Path, bool useImageSharpBugWorkaround = true)
        {
            return Task.Run(() =>
            {
                return AreImagesEqual(image1Path, image2Path, useImageSharpBugWorkaround);
            });
        }

        private static string GetRotatedAmount(Image<Rgba32> source)
        {
            var blah = source?.MetaData?.ExifProfile?.GetValue(ExifTag.Orientation);
            if (blah == null)
            {
                return "0";
            }

            var blahString = blah.ToString();

            var digits = blahString.Where(t => char.IsDigit(t)).ToArray();
            if (!digits.Any())
            {
                return "0";
            }
            return new string(digits);
        }

        private static Image<Rgba32> LoadImageHelper(string imagePath, List<string> tempFiles, bool useImageSharpBugWorkaround)
        {
            if (useImageSharpBugWorkaround && FileTypeHelper.IsJpgFile(imagePath))
            {
                var imageAsPngPath = ImageConverter.ConvertJpgToPngWithAutoRotate(imagePath);
                tempFiles.Add(imageAsPngPath);
                return Image.Load(imageAsPngPath);
            }
            else
            {
                return Image.Load(imagePath);
            }
        }

        private static bool AreImagesEqual(string image1Path, string image2Path, bool useImageSharpBugWorkaround)
        {
            if (!File.Exists(image1Path)) throw new FileNotFoundException("Could not find Image1", image1Path);
            if (!File.Exists(image2Path)) throw new FileNotFoundException("Could not find Image2", image2Path);

            var w = Stopwatch.StartNew();

            var tempFiles = new List<string>();

            try
            {
                using (var image1 = LoadImageHelper(image1Path, tempFiles, useImageSharpBugWorkaround))
                using (var image2 = LoadImageHelper(image2Path, tempFiles, useImageSharpBugWorkaround))
                {
                    image1.Mutate(t => t.AutoOrient());
                    image2.Mutate(t => t.AutoOrient());

                    long pixelsWrong = 0;

                    if (image1.Frames.Count > 1)
                    {
                        Console.WriteLine("Image with multiple frames detected.");
                        Console.WriteLine($"Comparing all {image1.Frames.Count} frames...");

                        int pointer1 = 0;
                        int pointer2 = 0;

                        //using (ImageFrame<Rgba32> frame1FromImage = new ImageFrame<Rgba32>(image1))
                        //using (ImageFrame<Rgba32> frame2FromImage = new ImageFrame<Rgba32>(image2))
                        //{
                        //var imageFrames1 = new ImageFrame<Rgba32>[image1.Frames.Count + 1];
                        //var imageFrames2 = new ImageFrame<Rgba32>[image2.Frames.Count + 1];

                        //frame1FromImage.MetaData.FrameDelay = image1.MetaData.FrameDelay;
                        //frame2FromImage.MetaData.FrameDelay = image2.MetaData.FrameDelay;

                        //imageFrames1[0] = frame1FromImage;
                        //imageFrames2[0] = frame2FromImage;

                        //for (int i = 0; i < image1.Frames.Count; i++)
                        //{
                        //    imageFrames1[i + 1] = image1.Frames[i];
                        //}
                        //for (int i = 0; i < image2.Frames.Count; i++)
                        //{
                        //    imageFrames2[i + 1] = image2.Frames[i];
                        //}

                        //This code outputs all gif frames as png images
                        //for (int i = 0; i < imageFrames1.Length; i++)
                        //{
                        //    using (var im = new Image<Rgba32>(imageFrames1[i]))
                        //    {
                        //        using (var fs = new FileStream($"1_{i}.png", FileMode.Create))
                        //        {
                        //            im.SaveAsPng(fs);
                        //        }
                        //    }
                        //}

                        //for (int i = 0; i < imageFrames2.Length; i++)
                        //{
                        //    using (var im = new Image<Rgba32>(imageFrames2[i]))
                        //    {
                        //        using (var fs = new FileStream($"2_{i}.png", FileMode.Create))
                        //        {
                        //            im.SaveAsPng(fs);
                        //        }
                        //    }
                        //}



                        ImageFrame<Rgba32> frame1 = image1.Frames[0];
                        ImageFrame<Rgba32> frame2 = image2.Frames[0];

                        //TODO: Fix uncomment top line when ImageSharp fixes bug where FrameDelay of frame[0] is always 0
                        int delay1 = frame1.MetaData.FrameDelay;
                        int delay2 = frame2.MetaData.FrameDelay;
                        //int delay1 = image1.MetaData.FrameDelay;
                        //int delay2 = image2.MetaData.FrameDelay;

                        while (true)
                        {
                            pixelsWrong += FindWrongPixels(frame1, frame2);

                            //This code outputs the incorrect frames as an image
                            if (pixelsWrong != 0)
                            {
                                //Currently disabled
                                //TODO: When image sharp implements this again, fix it.

                                //using (var im = new Image<Rgba32>(frame1))
                                //{
                                //    using (var fs = new FileStream($"Wrong_1_{pointer1}.png", FileMode.Create))
                                //    {
                                //        im.SaveAsPng(fs);
                                //    }
                                //}
                                //using (var im = new Image<Rgba32>(frame2))
                                //{
                                //    using (var fs = new FileStream($"Wrong_2_{pointer2}.png", FileMode.Create))
                                //    {
                                //        im.SaveAsPng(fs);
                                //    }
                                //}
                            }

                            var min = Math.Min(delay1, delay2);
                            delay1 -= min;
                            delay2 -= min;

                            if (delay1 == 0)
                            {
                                pointer1++;

                                if (pointer1 < image1.Frames.Count)
                                {
                                    frame1 = image1.Frames[pointer1];
                                    delay1 += frame1.MetaData.FrameDelay;
                                }
                            }
                            if (delay2 == 0)
                            {
                                pointer2++;

                                if (pointer2 < image2.Frames.Count)
                                {
                                    frame2 = image2.Frames[pointer2];
                                    delay2 += frame2.MetaData.FrameDelay;
                                }
                            }

                            if (pointer1 > image1.Frames.Count && pointer2 > image2.Frames.Count)
                            {
                                //Same number of frames
                                break;
                            }
                            else if (pointer1 <= image1.Frames.Count && pointer2 <= image2.Frames.Count)
                            {
                                //Just continue;
                            }
                            else
                            {
                                //Incorrect frame number
                                Console.WriteLine("Number of frames is not correct");
                                if (pixelsWrong == 0)
                                {
                                    pixelsWrong += 1; //Just to be sure it fails
                                }
                                break;
                            }
                        }
                    }

                    else
                    {
                        pixelsWrong = FindWrongPixels(image1.Frames[0], image2.Frames[0]);
                    }

                    Console.WriteLine($"Image comparison done in: {w.Elapsed}. Wrong pixels: {pixelsWrong}");
                    if (pixelsWrong > 0)
                    {
                        return false;
                    }

                    return true;
                }
            }
            finally
            {
                foreach (var tempFile in tempFiles)
                {
                    FileHelperMethods.SafeDeleteTempFile(tempFile);
                }
            }
        }

        private static long FindWrongPixels(ImageFrame<Rgba32> image1, ImageFrame<Rgba32> image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return image1.Width * image1.Height;
            }

            int width = image1.Width;
            int height = image1.Height;

            long pixelsWrong = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel1 = image1[x, y];
                    var pixel2 = image2[x, y];

                    if (pixel1 != pixel2)
                    {
                        if (pixel1.A == 0 && pixel2.A == 0)
                        {
                            //Optimization that happens to better be able to compress png's sometimes
                            //Fully transparent pixel so the pixel is still the same no matter what the RGB values
                        }
                        else
                        {
                            //Console.WriteLine($"Failed, elapsed time: {w.Elapsed}");
                            //return false;
                            pixelsWrong++;
                        }
                    }
                }
            }

            return pixelsWrong;
        }
    }
}
