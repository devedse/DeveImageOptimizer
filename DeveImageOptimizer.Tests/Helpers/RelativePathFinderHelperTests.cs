using DeveImageOptimizer.Helpers;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{
    public class RelativePathFinderHelperTests
    {
        [Fact]
        public void MakesARelativePath()
        {
            var dirPath = @"C:\WOP\DeveMazeGenerator";
            var filePath = @"C:\WOP\DevemazeGenerator\Images\TestImage.png";

            var expectedPath = @"Images/TestImage.png";

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }

        [Fact]
        public void WorksForNullPath()
        {
            string dirPath = null;
            var filePath = @"C:\WOP\DevemazeGenerator\Images\TestImage.png";

            var expectedPath = filePath;

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }
    }
}
