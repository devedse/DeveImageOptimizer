using DeveImageOptimizer.Helpers;
using System;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{
    public class RelativePathFinderHelperTests
    {
        [Fact]
        public void MakesARelativePath()
        {
            string dirPath;
            string filePath;
            if (OperatingSystem.IsWindows())
            {
                dirPath = @"C:\WOP\DeveMazeGenerator";
                filePath = @"C:\WOP\DevemazeGenerator\Images\TestImage.png";
            }
            else
            {
                dirPath = @"/home/WOP/DeveMazeGenerator";
                filePath = @"/home/WOP/DeveMazeGenerator/Images/TestImage.png";
            }

            var expectedPath = @"Images/TestImage.png";

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }

        [Fact]
        public void WorksForNullPath()
        {
            string dirPath = null;
            string filePath;
            if (OperatingSystem.IsWindows())
            {
                filePath = @"C:\WOP\DevemazeGenerator\Images\TestImage.png";
            }
            else
            {
                filePath = @"/home/WOP/DeveMazeGenerator/Images/TestImage.png";
            }

            var expectedPath = filePath;

            var relativePath = RelativePathFinderHelper.GetRelativePath(dirPath, filePath);
            Assert.Equal(expectedPath, relativePath);
        }
    }
}
