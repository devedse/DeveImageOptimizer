using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.ExternalTools;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessingDirect
{
    public class FileOptimizerProcessorNewFacts
    {
        [SkippableFact]
        public async Task CorrectlyOptimizesLandscapeImage()
        {
            await OptimizeFileTest("Image1A.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesStandingRotatedImage()
        {
            await OptimizeFileTest("Image2A.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesPexelsPhoto()
        {
            await OptimizeFileTest("pexels-photo.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesVimPicture()
        {
            await OptimizeFileTest("vim16x16_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGifImage()
        {
            await OptimizeFileTest("devtools-full_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesVersioningImage()
        {
            await OptimizeFileTest("versioning-1_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesTimelineSelectorImage()
        {
            await OptimizeFileTest("TimelineSelector.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesSnakeImage()
        {
            await OptimizeFileTest("snake.png");
        }

        [SkippableFact, Trait(TraitNames.FailingTest, Trait.True)]
        public async Task CorrectlyOptimizesCraDynamicImportImage()
        {
            await OptimizeFileTest("cra-dynamic-import_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesDevtoolsSidePaneImage()
        {
            await OptimizeFileTest("devtools-side-pane_1.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage1()
        {
            await OptimizeFileTest("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage2()
        {
            await OptimizeFileTest("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage3()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpImage4()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGifSourceImage()
        {
            await OptimizeFileTest("Source.gif");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesIconImage()
        {
            await OptimizeFileTest("icon_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesGoProImage()
        {
            await OptimizeFileTest("GoProBison.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesSmileFaceBmpImage()
        {
            await OptimizeFileTest("SmileFaceBmp.bmp");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesPezImageWithSpaceInName()
        {
            await OptimizeFileTest("pez image with space.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesAfterJpgImage()
        {
            await OptimizeFileTest("After.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesRuthImage()
        {
            //As of 31-10-2018: This is the only failing test
            await OptimizeFileTest("baberuth_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesAlreadyOptimizedOwl()
        {
            await OptimizeFileTest("AlreadyOptimizedOwl.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesOfficeLensImage()
        {
            await OptimizeFileTest("OfficeLensImage.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesImageSharpGeneratedImage()
        {
            await OptimizeFileTest("Generated_0.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes01png()
        {
            await OptimizeFileTest("01.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes03png()
        {
            await OptimizeFileTest("03.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes031png()
        {
            await OptimizeFileTest("03_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes05png()
        {
            await OptimizeFileTest("05.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes05_3png()
        {
            await OptimizeFileTest("05_3.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizes08_01png()
        {
            await OptimizeFileTest("08_1.png");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesBrokenstickmanJpg()
        {
            await OptimizeFileTest("Brokenstickman.JPG");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesSmileEnzoImage()
        {
            await OptimizeFileTest("SmileEnzo.jpg");
        }

        [SkippableFact]
        public async Task OptimizesMonoGameIcon()
        {
            await OptimizeFileTest("MonoGameIcon.bmp");
        }

        [SkippableFact]
        public async Task Optimizes1399751938250JpgImage()
        {
            await OptimizeFileTest("1399751938250.jpg");
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesReadOnlyFile()
        {
            var fileName = "ReadOnlyJpg.jpg";
            var filePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);

            new FileInfo(filePath).IsReadOnly = true;

            await OptimizeFileTest(fileName);
        }

        [SkippableFact]
        public async Task CorrectlyOptimizesBlockedFile()
        {
            var fileName = "BlockedJpg.jpg";
            var filePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);

            if (OperatingSystem.IsWindows())
            {
                using (var zoneIdentifier = new ZoneIdentifier(filePath))
                {
                    zoneIdentifier.Zone = UrlZone.Internet;
                }
            }

            await OptimizeFileTest(fileName);
        }

        private async Task OptimizeFileTest(string fileName)
        {
            var config = ConfigCreator.CreateTestConfig(true, directMode: true);

            var fop = new FileOptimizerProcessor(config);
            var image1path = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            try
            {
                var fileToOptimize = new OptimizableFile(image1temppath, null, new FileInfo(image1temppath).Length);

                await fop.OptimizeFile(fileToOptimize);

                Assert.Equal(OptimizationResult.Success, fileToOptimize.OptimizationResult);

                var fileOptimized = new FileInfo(image1temppath);
                var fileUnoptimized = new FileInfo(image1path);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimized.Length <= fileUnoptimized.Length);
            }
            finally
            {
                File.Delete(image1temppath);
            }
        }
    }
}
