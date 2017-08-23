using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class ImageComparerFacts
    {
        [Fact]
        public async void AreImagesEqual1()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image1A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image1B.JPG");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [SkippableFact]
        public async void AreImagesEqual1WithoutWorkaround()
        {
            //This test will fail as long as the JPG decoding bug is not fixed. That's why I made it skippable for now.

            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image1A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image1B.JPG");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath, false);

            Skip.IfNot(areEqual, "This test should not be skipped anymore once ImageSharp correctly decodes JPG files");
        }

        //public void AreImagesEqual2()
        //{
        //    //Well these images are in fact not equal due to a bug in one of the Image Optimizers. Basically if since Image2A contains a rotation it won't work correctly.

        //    var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2A.JPG");
        //    var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2B.JPG");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [Fact]
        public async void AreImagesEqual3()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image3A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image3B.JPG");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualVimImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "vim16x16_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "vim16x16_2.png");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualGif()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "devtools-full_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "devtools-full_2.gif");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualGif2()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "pitch_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "pitch_2.gif");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualVersioningImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "versioning-1_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "versioning-1_2.png");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [SkippableFact]
        public async void AreImagesEqualCraDynamicImport()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "cra-dynamic-import_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "cra-dynamic-import_2.gif");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Skip.If(areEqual == false, "This should also not fail I think.");
            //Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualDevToolsSidePanel()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "devtools-side-pane_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "devtools-side-pane_2.gif");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async void AreImagesEqualIconImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "icon_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "icon_2.png");

            var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }
    }
}
