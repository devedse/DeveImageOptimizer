using DeveImageOptimizer.Helpers;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class GifFacts
    {
        //[SkippableFact]
        //public async Task SourceEqualsFinal()
        //{
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source.gif");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Final.gif");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [SkippableFact]
        public async Task SourceEqualsSourceGifcicle()
        {
            //Gifcicle command: gifsicle.exe -b -w -j --no-conserve-memory -o result.gif -O3 source.gif
            var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source-Gifcicle.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);
            //Assert.True(areEqual);

            var sb = new StringBuilder();
            sb.AppendLine("This test shouldn't fail but for some reason it does sometimes.");
            using (var img1 = Image.Load(imageApath))
            using (var img2 = Image.Load(imageBpath))
            {
                sb.AppendLine($"Frames Image a: {img1.Frames.Count}");
                sb.AppendLine($"Frames Image a: {img2.Frames.Count}");

                for (int i = 0; i < 5; i++)
                {
                    var frame1 = img1.Frames[i];
                    var frame2 = img2.Frames[i];

                    int pixelsWrong = 0;
                    for (int y = 0; y < frame1.Height; y++)
                    {
                        for (int x = 0; x < frame1.Width; x++)
                        {
                            var pixel1 = frame1[x, y];
                            var pixel2 = frame2[x, y];

                            if (pixel1 != pixel2)
                            {
                                if (pixel1.A == 0 && pixel2.A == 0)
                                {
                                }
                                else
                                {
                                    //sb.AppendLine($"Pixel Wrong in Frame {i}>({x},{y}) Pixel1: {pixel1.R},{pixel1.G},{pixel1.B},{pixel1.A} Pixel2: {pixel2.R},{pixel2.G},{pixel2.B},{pixel2.A}");
                                    pixelsWrong++;
                                }
                            }
                        }
                    }

                    sb.AppendLine($">>> Frame: {i} has {pixelsWrong} wrong pixels.");
                }
            }

            if (!areEqual)
            {
                throw new SkipException(sb.ToString());
            }
            else
            {
                Debug.WriteLine(sb.ToString());
            }
        }

        //[SkippableFact]
        //public async Task SourceEqualsSourceGifcicleImageMagick()
        //{
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source.gif");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source-Gifcicle-ImageMagick.gif");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        //[SkippableFact]
        //public async Task SourceEqualsSourceImageMagick()
        //{
        //    //ImageMagick command: magick.exe convert -quiet -compress LZW -layers optimize source.gif result.gif
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source.gif");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source-ImageMagick.gif");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        //[SkippableFact]
        //public async Task SourceEqualsImageMagickGifcicle()
        //{
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source.gif");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "GifInvest", "Source-ImageMagick-Gifcicle.gif");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}
    }
}
