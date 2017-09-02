using DeveImageOptimizer.Helpers;
using System.IO;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{
    public class FileHashCalculatorTests
    {
        [Fact]
        public void FileHashIsCorrect()
        {
            var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            
            var hash = FileHashCalculator.CalculateFileHash(imageApath);

            var expected = "697314CF9BE8B7431BC67358D5F3A0BEA49FD835E086BF2861E41757A61D68F8E71AACBE430DC10C9765FD2C9BDA665B36E0BCD9FAEEAF69BCCA8337FE68FAF2";
            Assert.Equal(expected, hash);
        }
    }
}
