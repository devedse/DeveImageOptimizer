﻿using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.ImageOperations
{
    public class ImageExifComparerFacts
    {
        [Fact]
        public async Task FindsOutThatExifDataIsTheSame()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithSameExif.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task FindsOutThatExifDataIsDifferent()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithDifferentExif.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.False(areEqual);
        }
    }
}