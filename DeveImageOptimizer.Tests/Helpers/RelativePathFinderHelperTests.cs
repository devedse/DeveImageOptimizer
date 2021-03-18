using DeveImageOptimizer.Helpers;
using System.IO;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{
    public class RelativePathFinderHelperTests
    {
        [Fact]
        public void MakesARelativePath()
        {
            var s = Path.DirectorySeparatorChar;

            var dirPath = $"C:{s}WOP{s}DeveMazeGenerator";
            var filePath = $"C:{s}WOP{s}DevemazeGenerator{s}Images{s}TestImage.png";

            var expectedPath = @"Images/TestImage.png";

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }

        [Fact]
        public void WorksForNullPath()
        {
            var s = Path.DirectorySeparatorChar;
            string dirPath = null;
            var filePath = $"C:{s}WOP{s}DevemazeGenerator{s}Images{s}TestImage.png";

            var expectedPath = filePath;

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }
    }
}
