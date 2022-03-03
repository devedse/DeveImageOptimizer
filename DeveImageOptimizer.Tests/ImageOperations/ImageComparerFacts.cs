using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.ImageOperations
{
    public class ImageComparerFacts
    {
        [Fact]
        public async Task FindsOutThatImageIsTheSameForBothExifAndPixels()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithSameExif.JPG");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task PngImageStillComparesCorrectly()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "08_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "08_1.png");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task BmpImageStillComparesCorrectly()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "SmileFaceBmp.bmp");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "SmileFaceBmp.bmp");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task GifImageStillComparesCorrectly()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-full_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-full_1.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }
    }
}
