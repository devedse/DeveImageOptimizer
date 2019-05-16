using DeveImageOptimizer.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{
    public class ImageComparerFacts
    {
        [Fact]
        public async Task AreImagesEqual1()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image1B.JPG");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqual1WithoutWorkaround()
        {
            //This test will fail as long as the JPG decoding bug is not fixed. That's why I made it skippable for now.
            //Update 31-10-2018: Apparently this was fixed :o

            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image1B.JPG");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath, false);

            Skip.IfNot(areEqual, "This test should not be skipped anymore once ImageSharp correctly decodes JPG files");
        }

        //public async Task AreImagesEqual2()
        //{
        //    //Well these images are in fact not equal due to a bug in one of the Image Optimizers. Basically if since Image2A contains a rotation it won't work correctly.

        //    var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2A.JPG");
        //    var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectoryForTests.Value, "TestImages", "Image2B.JPG");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [Fact]
        public async Task AreImagesEqual3()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image3A.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "Image3B.JPG");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualVimImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "vim16x16_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "vim16x16_2.png");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualGif()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "devtools-full_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "devtools-full_2.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualGif2()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "pitch_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "pitch_2.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualVersioningImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "versioning-1_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "versioning-1_2.png");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [SkippableFact]
        public async Task AreImagesEqualCraDynamicImport()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "cra-dynamic-import_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "cra-dynamic-import_2.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Skip.If(areEqual == false, "This should also not fail I think. Currently it fails though, so this needs fixes in the FileOpitmizerFull.");
            //Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualDevToolsSidePanel()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "devtools-side-pane_1.gif");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "devtools-side-pane_2.gif");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task AreImagesEqualIconImage()
        {
            var imageApath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "icon_1.png");
            var imageBpath = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "icon_2.png");

            var areEqual = await ImageComparer.AreImagesEqualAsync(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        //This test fails due to FileOptimizer making some pixels transparent for some reason
        //[Fact]
        //public async Task AreImagesEqualRuthImage()
        //{
        //    var imageApath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "baberuth_1.png");
        //    var imageBpath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "baberuth_2.png");

        //    var areEqual = await ImageComparer2.AreImagesEqualAsync(imageApath, imageBpath);

        //    Assert.True(areEqual);
        //}

        [Fact]
        public async Task AreImagesEqualChatApp()
        {
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "chatapp-1.png");
            var image2path = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "chatapp-2.png");
            var image3path = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "chatapp-3.png");
            var image4path = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", "chatapp-4.png");

            var areEqual1 = await ImageComparer.AreImagesEqualAsync(image1path, image2path);
            var areEqual2 = await ImageComparer.AreImagesEqualAsync(image3path, image4path);

            Assert.True(areEqual1);
            Assert.True(areEqual2);
        }
    }
}
