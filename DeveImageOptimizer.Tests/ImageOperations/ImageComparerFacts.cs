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
    }
}
