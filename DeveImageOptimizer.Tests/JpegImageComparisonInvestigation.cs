using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageConversion;
using ImageSharp;
using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class JpegImageComparisonInvestigation
    {
        //[Fact]
        //public void CompareWithImage1A()
        //{
        //    Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1B.JPG");

        //    CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_JPG");
        //}

        //[Fact]
        //public void CompareWithImage1AConverted()
        //{

        //    Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.PNG");

        //    var a = ImageConverter.ConvertJpgToPng(image1path);
        //    CompareTheseImagesAndWriteResultToOutput(a, image2path, "JPGWITHVIPS_TO_VIPS");
        //}

        //[Fact]
        //public void CompareWithImage1B()
        //{
        //    Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1B.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

        //    CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG1B_TO_VIPS");
        //}

        [Fact]
        public void ImageSharpToLibVips()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_VIPS");
        }

        [Fact]
        public void ImageSharpToPaint()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_Paint.png");

            CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_PAINT");
        }

        [Fact]
        public void ImageSharpToPaintNET()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_PaintNET");
        }

        [Fact]
        public void ImageSharpToIrfanView()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");

            CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_IRFANVIEW");
        }




        [Fact]
        public void PaintToVips()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_Paint.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "PAINT_TO_VIPS"));
        }

        [Fact]
        public void VipsToPaintNet()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "VIPS_TO_PAINTNET"));
        }

        [Fact]
        public void PaintNetToIrfanView()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "PAINTNET_TO_IRFANVIEW"));
        }

        [Fact]
        public void IrfanViewToPaint()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_Paint.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "IRFANVIEW_TO_PAINT"));
        }


        //Other 2
        [Fact]
        public void IrfanViewToVips()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            //IrfanView and LibVIPS have the same decoder
            Assert.Equal(0, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "IRFANVIEW_TO_VIPS"));
        }

        [Fact]
        public void PaintToPaintNET()
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);
            var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_Paint.png");
            var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            //IrfanView and LibVIPS have the same decoder
            Assert.Equal(0, CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "PAINT_TO_PAINTNET"));
        }

        private int CompareTheseImagesAndWriteResultToOutput(string image1, string image2, string outputImageName)
        {
            var outputDir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "JpegInvestigation");
            Directory.CreateDirectory(outputDir);

            var outputImage = Path.Combine(outputDir, outputImageName + ".png");
            var outputImage2 = Path.Combine(outputDir, outputImageName + "2.png");

            int pixelsWrong = 0;

            using (var img1 = Image.Load(image1))
            using (var img2 = Image.Load(image2))
            {
                Assert.Equal(img1.Width, img2.Width);
                Assert.Equal(img1.Height, img2.Height);

                using (var outputImg = new Image<Rgba32>(img1.Width, img1.Height))
                using (var outputImg2 = new Image<Rgba32>(img1.Width, img1.Height))
                {

                    for (int y = 0; y < img1.Height; y++)
                    {
                        for (int x = 0; x < img1.Width; x++)
                        {
                            var pixel1 = img1[x, y];
                            var pixel2 = img2[x, y];

                            if (pixel1 != pixel2)
                            {
                                outputImg[x, y] = new Rgba32(255, 0, 0);
                                outputImg2[x, y] = new Rgba32(255, 0, 0);
                                pixelsWrong++;
                            }
                            else
                            {
                                outputImg[x, y] = pixel1;
                                outputImg2[x, y] = new Rgba32(255, 255, 255);
                            }
                        }
                    }

                    using (var fs = new FileStream(outputImage, FileMode.Create))
                    {
                        outputImg.SaveAsPng(fs);
                    }

                    using (var fs = new FileStream(outputImage2, FileMode.Create))
                    {
                        outputImg2.SaveAsPng(fs);
                    }
                }
            }

            Console.WriteLine($"Pixels wrong: {pixelsWrong}");
            return pixelsWrong;
        }
    }
}