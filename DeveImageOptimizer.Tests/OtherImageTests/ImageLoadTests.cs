using DeveImageOptimizer.Helpers;
using ExifLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Linq;
using Xunit;

namespace DeveImageOptimizer.Tests.OtherImageTests
{
    public class ImageLoadTests
    {
        [Fact]
        public void CorrectlyLoadsLandscapeImage()
        {
            TestImageLoading("Image1A.JPG");
        }

        [Fact]
        public void CorrectlyLoadsStandingRotatedImage()
        {
            TestImageLoading("Image2A.JPG");
        }

        [Fact]
        public void CorrectlyLoadsPexelsPhoto()
        {
            TestImageLoading("pexels-photo.jpg");
        }

        [Fact]
        public void CorrectlyLoadsVimPicture()
        {
            TestImageLoading("vim16x16_1.png");
        }

        [Fact]
        public void CorrectlyLoadsGifImage()
        {
            TestImageLoading("devtools-full_1.gif");
        }

        [Fact]
        public void CorrectlyLoadsVersioningImage()
        {
            TestImageLoading("versioning-1_1.png");
        }

        [Fact]
        public void CorrectlyLoadsSnakeImage()
        {
            TestImageLoading("snake.png");
        }

        [Fact]
        public void CorrectlyLoadsCraDynamicImportImage()
        {
            TestImageLoading("cra-dynamic-import_1.gif");
        }

        [Fact]
        public void CorrectlyLoadsDevtoolsSidePaneImage()
        {
            TestImageLoading("devtools-side-pane_1.gif");
        }

        [Fact]
        public void CorrectlyLoadsImageSharpImage1()
        {
            TestImageLoading("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [Fact]
        public void CorrectlyLoadsImageSharpImage2()
        {
            TestImageLoading("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [Fact]
        public void CorrectlyLoadsImageSharpImage3()
        {
            TestImageLoading("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [Fact]
        public void CorrectlyLoadsImageSharpImage4()
        {
            TestImageLoading("Imagesharp/ResizeWithPadMode_Rgba32_CalliphoraPartial.png");
        }

        [Fact]
        public void CorrectlyLoadsGifSourceImage()
        {
            TestImageLoading("Source.gif");
        }

        [Fact]
        public void CorrectlyLoadsIconImage()
        {
            TestImageLoading("icon_1.png");
        }

        [Fact]
        public void CorrectlyLoadsGoProImage()
        {
            TestImageLoading("GoProBison.JPG");
        }

        [Fact]
        public void CorrectlyLoadsSmileFaceBmpImage()
        {
            TestImageLoading("SmileFaceBmp.bmp");
        }

        [Fact]
        public void CorrectlyLoadsPezImageWithSpaceInName()
        {
            TestImageLoading("pez image with space.jpg");
        }

        [Fact]
        public void CorrectlyLoadsAfterJpgImage()
        {
            TestImageLoading("After.JPG");
        }

        [Fact]
        public void CorrectlyLoadsRuthImage()
        {
            //As of 31-10-2018: This is the only failing test
            TestImageLoading("baberuth_1.png");
        }

        [Fact]
        public void CorrectlyLoadsAlreadyOptimizedOwl()
        {
            TestImageLoading("AlreadyOptimizedOwl.jpg");
        }

        [Fact]
        public void CorrectlyLoadsOfficeLensImage()
        {
            TestImageLoading("OfficeLensImage.jpg");
        }

        [Fact]
        public void CorrectlyLoadsImageSharpGeneratedImage()
        {
            TestImageLoading("Generated_0.jpg");
        }

        [Fact]
        public void CorrectlyLoads01png()
        {
            TestImageLoading("01.png");
        }

        [Fact]
        public void CorrectlyLoads03png()
        {
            TestImageLoading("03.png");
        }

        [Fact]
        public void CorrectlyLoads031png()
        {
            TestImageLoading("03_1.png");
        }

        [Fact]
        public void CorrectlyLoads05png()
        {
            TestImageLoading("05.png");
        }

        [Fact]
        public void CorrectlyLoads05_3png()
        {
            TestImageLoading("05_3.png");
        }

        [Fact]
        public void CorrectlyLoads08_01png()
        {
            TestImageLoading("08_1.png");
        }

        [Fact]
        public void CorrectlyLoadsBrokenstickmanJpg()
        {
            TestImageLoading("Brokenstickman.JPG");
        }

        [Fact]
        public void CorrectlyLoadsMonoGameIcon()
        {
            TestImageLoading("MonoGameIcon.bmp");
        }

        private void TestImageLoading(string filename)
        {
            var imagepath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", filename);

            using (var img = Image.Load<Rgba32>(imagepath))
            {
                Assert.NotNull(img);
                Assert.NotEqual(0, img.Width);
                Assert.NotEqual(0, img.Height);
            }

            if (FileTypeHelper.IsJpgFile(imagepath))
            {
                var file = ImageFile.FromFile(imagepath);
                ExifProperty orientationExif = file.Properties.FirstOrDefault(t => t.Tag == ExifTag.Orientation);

                Assert.NotNull(file);
            }
        }
    }
}
