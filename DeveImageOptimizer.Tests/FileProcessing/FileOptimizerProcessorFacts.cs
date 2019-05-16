using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.FileProcessing
{
    public class FileOptimizerProcessorFacts
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

        private async Task OptimizeFileTest(string fileName)
        {
            var fileOptimizerPath = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();

            var fop = new FileOptimizerProcessor(fileOptimizerPath, default, default, TestConstants.ShouldShowFileOptimizerWindow);
            var image1path = Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, "TestImages", fileName);
            var tempfortestdir = Path.Combine(FolderHelperMethods.TempDirectory.Value, "TempForTest");
            var image1temppath = Path.Combine(tempfortestdir, RandomFileNameHelper.RandomizeFileName(fileName));

            Directory.CreateDirectory(tempfortestdir);
            File.Copy(image1path, image1temppath, true);

            try
            {
                var result = await fop.OptimizeFile(image1temppath, null);

                Assert.Equal(OptimizationResult.Success, result.OptimizationResult);

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
