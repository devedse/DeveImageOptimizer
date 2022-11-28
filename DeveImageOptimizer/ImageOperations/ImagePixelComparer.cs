using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.IO;

namespace DeveImageOptimizer.ImageOperations
{
    public static class ImagePixelComparer
    {
        private static Image<Rgba64> LoadImageHelper(string imagePath)
        {
            return Image.Load<Rgba64>(imagePath);
        }

        public static bool AreImagePixelsEqual(string image1Path, string image2Path)
        {
            if (!File.Exists(image1Path))
            {
                throw new FileNotFoundException("Could not find Image1", image1Path);
            }

            if (!File.Exists(image2Path))
            {
                throw new FileNotFoundException("Could not find Image2", image2Path);
            }

            var w = Stopwatch.StartNew();

            using (var image1 = LoadImageHelper(image1Path))
            using (var image2 = LoadImageHelper(image2Path))
            {
                image1.Mutate(t => t.AutoOrient());
                image2.Mutate(t => t.AutoOrient());

                long pixelsWrong = 0;

                if (image1.Frames.Count > 1)
                {
                    Console.WriteLine("Image with multiple frames detected.");

                    var gifMetaData1 = image1.Metadata.GetGifMetadata();
                    var gifMetaData2 = image2.Metadata.GetGifMetadata();

                    if (gifMetaData1 != null && gifMetaData2 != null)
                    {
                        if (gifMetaData1.RepeatCount != gifMetaData2.RepeatCount)
                        {
                            Console.WriteLine($"Repeat count for Gif not equal. Image1: {gifMetaData1.RepeatCount}, Image2: {gifMetaData2.RepeatCount}");
                            return false;
                        }
                    }

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



                    ImageFrame<Rgba64> frame1 = image1.Frames[0];
                    ImageFrame<Rgba64> frame2 = image2.Frames[0];

                    int delay1 = frame1.Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay;
                    int delay2 = frame2.Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay;

                    while (true)
                    {
                        pixelsWrong += FindWrongPixels(frame1, frame2);

                        //This code outputs the incorrect frames as an image
                        if (pixelsWrong != 0)
                        {
                            //Currently disabled

                            //var data1 = new Rgba32[frame1.Width * frame1.Height];
                            //var span1 = new Span<Rgba32>(data1);
                            //frame1.CopyPixelDataTo(span1);

                            //var data2 = new Rgba32[frame2.Width * frame2.Height];
                            //var span2 = new Span<Rgba32>(data2);
                            //frame2.CopyPixelDataTo(span1);

                            //using (var im = Image.LoadPixelData(data1, frame1.Width, frame1.Height))
                            //{
                            //    using (var fs = new FileStream($"Wrong_1_{pointer1}.png", FileMode.Create))
                            //    {
                            //        im.SaveAsPng(fs);
                            //    }
                            //}
                            //using (var im = Image.LoadPixelData(data2, frame2.Width, frame2.Height))
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
                                delay1 += frame1.Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay;
                            }
                        }
                        if (delay2 == 0)
                        {
                            pointer2++;

                            if (pointer2 < image2.Frames.Count)
                            {
                                frame2 = image2.Frames[pointer2];
                                delay2 += frame2.Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay;
                            }
                        }

                        if (pointer1 > image1.Frames.Count && pointer2 > image2.Frames.Count)
                        {
                            //Same number of frames
                            break;
                        }
                        else if (pointer1 <= image1.Frames.Count && pointer2 <= image2.Frames.Count)
                        {
                            //Just continue
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

                Console.WriteLine($"Image comparison done in: {w.Elapsed}. Wrong pixels: {pixelsWrong}. SourceFile: {image1Path}. ResultFile: {image2Path}.");
                if (pixelsWrong > 0)
                {
                    return false;
                }

                return true;
            }
        }

        private static long FindWrongPixels(ImageFrame<Rgba64> image1, ImageFrame<Rgba64> image2)
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
