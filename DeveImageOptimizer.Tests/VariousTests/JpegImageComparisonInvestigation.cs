using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class JpegImageComparisonInvestigation
    {
        public const string JpegInvestigationName = "JpegImageComparisonInvestigation";

        //[Fact]
        //public async Task CompareWithImage1A()
        //{
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1B.JPG");

        //    await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG_TO_JPG");
        //}

        //[Fact]
        //public async Task CompareWithImage1AConverted()
        //{
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1A.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.PNG");

        //    var a = ImageConverter.ConvertJpgToPng(image1path);
        //    await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutput(a, image2path, "JPGWITHVIPS_TO_VIPS");
        //}

        //[Fact]
        //public async Task CompareWithImage1B()
        //{;
        //    var image1path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1B.JPG");
        //    var image2path = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

        //    await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutput(image1path, image2path, "JPG1B_TO_VIPS");
        //}

        [Fact]
        public async Task ImageSharpToLibVips()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "JPG_TO_VIPS");
            //It doesn't really matter if this test fails but I just wanted to note this down to know if ImageSharp changes again
            Assert.Equal(957775, result);
        }

        [Fact]
        public async Task ImageSharpToIrfanView()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");

            var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "JPG_TO_IRFANVIEW");
            //It doesn't really matter if this test fails but I just wanted to note this down to know if ImageSharp changes again
            Assert.Equal(957775, result);
        }

        [Fact]
        public async Task ImageSharpToPaint()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_Paint.png");

            var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "JPG_TO_PAINT");
            //It doesn't really matter if this test fails but I just wanted to note this down to know if ImageSharp changes again
            Assert.Equal(918501, result);
        }

        [Fact]
        public async Task ImageSharpToPaintNET()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1A.JPG");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "JPG_TO_PaintNET");
            //It doesn't really matter if this test fails but I just wanted to note this down to know if ImageSharp changes again
            Assert.Equal(918501, result);
        }






        [Fact]
        public async Task PaintToVips()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_Paint.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "PAINT_TO_VIPS"));
        }

        [Fact]
        public async Task VipsToPaintNet()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "VIPS_TO_PAINTNET"));
        }

        [Fact]
        public async Task PaintNetToIrfanView()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "PAINTNET_TO_IRFANVIEW"));
        }

        [Fact]
        public async Task IrfanViewToPaint()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_Paint.png");

            //Ideally this would be 0, but it seems Paint.NET/Paint use the same decoder and IrfanView/LibVIPS. So if we compare others, they have different results.
            Assert.Equal(845209, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "IRFANVIEW_TO_PAINT"));
        }


        //Other 2
        [Fact]
        public async Task IrfanViewToVips()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_IrfanView.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_LibVIPS.png");

            //IrfanView and LibVIPS have the same decoder
            Assert.Equal(0, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "IRFANVIEW_TO_VIPS"));
        }

        [Fact]
        public async Task PaintToPaintNET()
        {
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_Paint.png");
            var image2path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "Image1_PaintNET.png");

            //IrfanView and LibVIPS have the same decoder
            Assert.Equal(0, await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(image1path, image2path, JpegInvestigationName, "PAINT_TO_PAINTNET"));
        }
    }
}