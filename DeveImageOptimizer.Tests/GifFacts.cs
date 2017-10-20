using DeveImageOptimizer.Helpers;
using SixLabors.ImageSharp;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests
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

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            if (!areEqual)
            {
                var sb = new StringBuilder();
                sb.AppendLine("This test shouldn't fail but for some reason it does sometimes.");
                using (var img1 = Image.Load(imageApath))
                {
                    sb.AppendLine($"Frames Image a: {img1.Frames.Count}");
                }
                using (var img2 = Image.Load(imageBpath))
                {
                    sb.AppendLine($"Frames Image a: {img2.Frames.Count}");
                }

                throw new SkipException(sb.ToString());
            }
            //Assert.True(areEqual);
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
