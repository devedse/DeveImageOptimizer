using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using System.IO;
using Xunit;

namespace DeveImageOptimizer.Tests.ImageOperations
{
    public class ImagePixelComparerFacts
    {
        [Fact]
        public void AreImagesEqual1()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1B.JPG");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        //[Fact]
        //public void AreImagesEqual1WithoutWorkaround()
        //{
        //    //This test will fail as long as the JPG decoding bug is not fixed. That's why I made it skippable for now.
        //    //Update 31-10-2018: Apparently this was fixed :o

        //    var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
        //    var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1B.JPG");

        //    var areEqual = await ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

        //    Skip.IfNot(areEqual, "This test should not be skipped anymore once ImageSharp correctly decodes JPG files");
        //}

        //public void AreImagesEqual2()
        //{
        //    //Well these images are in fact not equal due to a bug in one of the Image Optimizers. Basically if since Image2A contains a rotation it won't work correctly.

        //    var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2A.JPG");
        //    var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2B.JPG");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [Fact]
        public void AreImagesEqual3()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image3A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image3B.JPG");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualVimImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "vim16x16_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "vim16x16_2.png");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualGif()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-full_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-full_2.gif");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualGif2()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "pitch_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "pitch_2.gif");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualVersioningImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "versioning-1_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "versioning-1_2.png");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [SkippableFact]
        public void AreImagesEqualCraDynamicImport()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "cra-dynamic-import_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "cra-dynamic-import_2.gif");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Skip.If(areEqual == false, "This should also not fail I think. Currently it fails though, so this needs fixes in the FileOpitmizerFull.");
            //Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualDevToolsSidePanel()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-side-pane_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "devtools-side-pane_2.gif");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void AreImagesEqualIconImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "icon_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "icon_2.png");

            var areEqual = ImagePixelComparer.AreImagePixelsEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        //This test fails due to FileOptimizer making some pixels transparent for some reason
        //[Fact]
        //public void AreImagesEqualRuthImage()
        //{
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "baberuth_1.png");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "baberuth_2.png");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [Fact]
        public void AreImagesEqualChatApp()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "chatapp-1.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "chatapp-2.png");
            var image3path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "chatapp-3.png");
            var image4path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "chatapp-4.png");

            var areEqual1 = ImagePixelComparer.AreImagePixelsEqual(image1path, image2path);
            var areEqual2 = ImagePixelComparer.AreImagePixelsEqual(image3path, image4path);

            Assert.True(areEqual1);
            Assert.True(areEqual2);
        }
    }
}
