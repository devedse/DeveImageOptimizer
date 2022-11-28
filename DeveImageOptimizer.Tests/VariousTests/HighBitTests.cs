using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class HighBitTests
    {
        [Fact]
        public async Task ImageComparerFiguresOutImagesAreDifferent()
        {
            int width = 100;
            int height = 100;
            using var img1 = new Image<Rgba64>(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ushort value = (ushort)(y * width + x);
                    img1[x, y] = new Rgba64(value, (ushort)(value * 2), (ushort)(value * 4), ushort.MaxValue);
                }
            }


            var tempFolderHighBit = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "HighBitTests");
            Directory.CreateDirectory(tempFolderHighBit);
            var original = Path.Combine(tempFolderHighBit, "16bitimage.png");
            var original2 = Path.Combine(tempFolderHighBit, "16bitimage2.png");
            var originalWithChange = Path.Combine(tempFolderHighBit, "16bitimage_change.png");

            img1.SaveAsPng(original);
            img1.SaveAsPng(original2);
            img1[5, 5] = new Rgba64(img1[5, 5].R, (ushort)(img1[5, 5].G + 1), img1[5, 5].B, img1[5, 5].A);
            img1.SaveAsPng(originalWithChange);

            var result1 = ImagePixelComparer.AreImagePixelsEqual(original, original2);
            var result2 = ImagePixelComparer.AreImagePixelsEqual(original, originalWithChange);

            Assert.True(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task CorrectlyOptimizesHighBitImage()
        {
            int width = 100;
            int height = 100;
            using var img1 = new Image<Rgba64>(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ushort value = (ushort)(y * width + x);
                    img1[x, y] = new Rgba64(value, (ushort)(value * 2), (ushort)(value * 4), ushort.MaxValue);
                }
            }


            var tempFolderHighBit = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "HighBitTests");
            Directory.CreateDirectory(tempFolderHighBit);
            var original = Path.Combine(tempFolderHighBit, "16bitimage.png");
            var originalWithChange = Path.Combine(tempFolderHighBit, "16bitimage_change.png");

            img1.SaveAsPng(original);
            img1[5, 5] = new Rgba64(img1[5, 5].R, (ushort)(img1[5, 5].G + 1), img1[5, 5].B, img1[5, 5].A);
            img1.SaveAsPng(originalWithChange);






            var config = ConfigCreator.CreateTestConfig(true, directMode: true);

            var fop = new FileOptimizerProcessor(config);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(original));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(original, image1temppath, true);

            try
            {
                var fileToOptimize = new OptimizableFile(image1temppath, null, new FileInfo(image1temppath).Length);

                await fop.OptimizeFile(fileToOptimize);

                Assert.Equal(OptimizationResult.Success, fileToOptimize.OptimizationResult);

                var fileOptimized = new FileInfo(image1temppath);
                var fileUnoptimized = new FileInfo(original);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimized.Length <= fileUnoptimized.Length);


                var result2 = ImagePixelComparer.AreImagePixelsEqual(original, originalWithChange);
                var result3 = ImagePixelComparer.AreImagePixelsEqual(original, image1temppath);
                var result4 = ImagePixelComparer.AreImagePixelsEqual(originalWithChange, image1temppath);

                using var imgOutput = Image.Load<Rgba64>(image1temppath);
                Assert.NotEqual(img1[5, 5].G, imgOutput[5, 5].G);
                Assert.Equal(img1[5, 5].G, imgOutput[5, 5].G + 1);

                Assert.False(result2);
                Assert.True(result3);
                Assert.False(result4);
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }
    }
}
