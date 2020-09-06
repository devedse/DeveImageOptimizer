using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.ExternalTools;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessingNewImageOptimization
{
    public class FileOptimizeSizeCompareFacts
    {
        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesLandscapeImage()
        {
            await OptimizeFileTest("Image1A.JPG");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesStandingRotatedImage()
        {
            await OptimizeFileTest("Image2A.JPG");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesPexelsPhoto()
        {
            await OptimizeFileTest("pexels-photo.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesVimPicture()
        {
            await OptimizeFileTest("vim16x16_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesGifImage()
        {
            await OptimizeFileTest("devtools-full_1.gif");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesVersioningImage()
        {
            await OptimizeFileTest("versioning-1_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesSnakeImage()
        {
            await OptimizeFileTest("snake.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesCraDynamicImportImage()
        {
            await OptimizeFileTest("cra-dynamic-import_1.gif");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesDevtoolsSidePaneImage()
        {
            await OptimizeFileTest("devtools-side-pane_1.gif");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesImageSharpImage1()
        {
            await OptimizeFileTest("Imagesharp/Resize_IsAppliedToAllFrames_Rgba32_giphy.gif");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesImageSharpImage2()
        {
            await OptimizeFileTest("Imagesharp/ResizeFromSourceRectangle_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesImageSharpImage3()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithBoxPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesImageSharpImage4()
        {
            await OptimizeFileTest("Imagesharp/ResizeWithPadMode_Rgba32_CalliphoraPartial.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesGifSourceImage()
        {
            await OptimizeFileTest("Source.gif");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesIconImage()
        {
            await OptimizeFileTest("icon_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesGoProImage()
        {
            await OptimizeFileTest("GoProBison.JPG");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesSmileFaceBmpImage()
        {
            await OptimizeFileTest("SmileFaceBmp.bmp");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesPezImageWithSpaceInName()
        {
            await OptimizeFileTest("pez image with space.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesAfterJpgImage()
        {
            await OptimizeFileTest("After.JPG");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesRuthImage()
        {
            //As of 31-10-2018: This is the only failing test
            await OptimizeFileTest("baberuth_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesAlreadyOptimizedOwl()
        {
            await OptimizeFileTest("AlreadyOptimizedOwl.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesOfficeLensImage()
        {
            await OptimizeFileTest("OfficeLensImage.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesImageSharpGeneratedImage()
        {
            await OptimizeFileTest("Generated_0.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes01png()
        {
            await OptimizeFileTest("01.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes03png()
        {
            await OptimizeFileTest("03.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes031png()
        {
            await OptimizeFileTest("03_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes05png()
        {
            await OptimizeFileTest("05.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes05_3png()
        {
            await OptimizeFileTest("05_3.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizes08_01png()
        {
            await OptimizeFileTest("08_1.png");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesBrokenstickmanJpg()
        {
            await OptimizeFileTest("Brokenstickman.JPG");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesSmileEnzoImage()
        {
            await OptimizeFileTest("SmileEnzo.jpg");
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesReadOnlyFile()
        {
            var fileName = "ReadOnlyJpg.jpg";
            var filePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);

            new FileInfo(filePath).IsReadOnly = true;

            await OptimizeFileTest(fileName);
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task CorrectlyOptimizesBlockedFile()
        {
            var fileName = "BlockedJpg.jpg";
            var filePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);

            using (var zoneIdentifier = new ZoneIdentifier(filePath))
            {
                zoneIdentifier.Zone = UrlZone.Internet;
            }

            await OptimizeFileTest(fileName);
        }

        private async Task OptimizeFileTest(string fileName, ImageOptimizationLevel imageOptimizationLevel = ImageOptimizationLevel.Maximum)
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var configFileOptimizer = new DeveImageOptimizerConfiguration()
            {
                HideFileOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                UseNewDeveImageOptimizer = true,
                LogLevel = 4,
                ImageOptimizationLevel = imageOptimizationLevel
            };
            var configNew = new DeveImageOptimizerConfiguration()
            {
                HideFileOptimizerWindow = !TestConstants.ShouldShowFileOptimizerWindow,
                UseNewDeveImageOptimizer = false,
                LogLevel = 4,
                ImageOptimizationLevel = imageOptimizationLevel
            };

            var fopFileOptimizer = new FileOptimizerProcessor(configFileOptimizer);
            var fopNew = new FileOptimizerProcessor(configNew);
            var imagePath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = FolderHelperMethods.Internal_TempForTestDirectory.Value;
            var imagetemppathFileOptimizer = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));
            var imagetemppathNew = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(imagePath, imagetemppathFileOptimizer, true);
            File.Copy(imagePath, imagetemppathNew, true);

            try
            {
                var fileToOptimizeFileOptimizer = new OptimizableFile(imagetemppathFileOptimizer, null, new FileInfo(imagetemppathFileOptimizer).Length);
                var fileToOptimizeNew = new OptimizableFile(imagetemppathNew, null, new FileInfo(imagetemppathFileOptimizer).Length);

                await fopFileOptimizer.OptimizeFile(fileToOptimizeFileOptimizer);
                await fopNew.OptimizeFile(fileToOptimizeNew);

                Assert.Equal(OptimizationResult.Success, fileToOptimizeFileOptimizer.OptimizationResult);
                Assert.Equal(OptimizationResult.Success, fileToOptimizeNew.OptimizationResult);

                var fileOptimizedFileOptimizer = new FileInfo(imagetemppathFileOptimizer);
                var fileUnoptimizedFileOptimizer = new FileInfo(imagePath);

                var fileOptimizedNew = new FileInfo(imagetemppathFileOptimizer);
                var fileUnoptimizedNew = new FileInfo(imagePath);

                //Verify that the new file is actually smaller
                Assert.True(fileOptimizedFileOptimizer.Length <= fileUnoptimizedFileOptimizer.Length);
                Assert.True(fileOptimizedNew.Length <= fileUnoptimizedNew.Length);

                Assert.Equal(fileOptimizedFileOptimizer.Length, fileOptimizedNew.Length);
            }
            finally
            {
                File.Delete(imagetemppathFileOptimizer);
            }
        }
    }
}
