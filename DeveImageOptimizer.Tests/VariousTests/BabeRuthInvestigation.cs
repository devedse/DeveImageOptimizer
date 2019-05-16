using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Tests.TestConfig;
using DeveImageOptimizer.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class BabeRuthInvestigation
    {
        public const string BabeRuthInvestigationName = "BaberuthInvestigation";

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task ShouldBeEqualToAllIterationsOfRuth()
        {
            //These tests fail due to bug in TruePNG, but it's disabled in FileOptimizerFull

            var original = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "BaberuthInvestigation", "Original.png");
            var otherImagesPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "BaberuthInvestigation");

            var failers = new Dictionary<string, int>();
            var succeeders = new List<string>();
            var allImages = Directory.GetFiles(otherImagesPath).Where(image => Path.GetExtension(image).Equals(".png", StringComparison.OrdinalIgnoreCase)).ToList();

            Assert.NotEmpty(allImages);

            foreach (var image in allImages)
            {
                if (Path.GetExtension(image).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(image);
                    var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(original, image, BabeRuthInvestigationName, fileNameWithoutExtension);

                    if (result == 0)
                    {
                        succeeders.Add(fileNameWithoutExtension);
                    }
                    else
                    {
                        failers.Add(fileNameWithoutExtension, result);
                    }
                }
            }

            if (failers.Any() || allImages.Count != succeeders.Count)
            {
                throw new SkipException("This test fails because of an issue in FileOptimizer");
            }
        }

        [SkippableFact, Trait(TraitNames.ShouldSkipForAppVeyor, TraitShouldSkipForAppVeyor.Yes)]
        public async Task TestTruePngDirectly()
        {
            //These tests fail due to bug in TruePNG, but it's disabled in FileOptimizerFull

            var bPNGCopyMetadata = true;
            var bPNGAllowLossy = false;

            var sFlags = "";
            var iLevel = Math.Min(9 * 3 / 9, 3) + 1;
            sFlags += "-o" + iLevel + " ";
            if (bPNGCopyMetadata)
            {
                sFlags += "-md keep all ";
            }
            else
            {
                sFlags += "-tz -md remove all -g0 ";
            }
            if (bPNGAllowLossy)
            {
                sFlags += "-l ";
            }

            var fileOptimizerExe = FileOptimizerFullExeFinder.GetFileOptimizerPathOrThrowSkipTestException();
            var rootDir = Path.GetDirectoryName(fileOptimizerExe);

            var possiblePaths = new List<string>();

            possiblePaths.Add(Path.Combine(rootDir, "Plugins64", "TruePNG.exe"));
            possiblePaths.Add(Path.Combine(rootDir, "Plugins32", "TruePNG.exe"));

            var existingTruePng = possiblePaths.FirstOrDefault(t => File.Exists(t));

            if (existingTruePng == null)
            {
                throw new SkipException($"FileOptimizerFull exe file can't be found. Expected locations: " + string.Join(' ', possiblePaths));
            }


            var outputDir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TruePngInvestigation");
            Directory.CreateDirectory(outputDir);

            var tmpOutputFile = Path.Combine(outputDir, "truepnginvest.png");
            if (File.Exists(tmpOutputFile))
            {
                File.Delete(tmpOutputFile);
            }
            var original = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", "BaberuthInvestigation", "Original.png");
            var arguments = $"{sFlags}/i0 /tz /quiet /y /out \"{tmpOutputFile}\" \"{original}\"";

            var processStartInfo = new ProcessStartInfo(existingTruePng, arguments);
            using (var proc = Process.Start(processStartInfo))
            {
                proc.WaitForExit();
            }

            var result = await ImageComparerAndWriteOutputDifferences.CompareTheseImagesAndWriteResultToOutputAsync(original, tmpOutputFile, BabeRuthInvestigationName, "TruePngDirectlyOutput");

            //If this test fails, TruePNG still has a bug in it
            //It is skipped on the build server though
            Assert.Equal(0, result);

            //This should be fixed in next FileOptimizerFull (Basically still a workaround though for a bug in TruePNG): https://sourceforge.net/p/nikkhokkho/tickets/58/
            //I've also sent an email to the creator of TruePNG

            //RunPlugin((unsigned int) iCount, "TruePNG (4/16)", (sPluginsDirectory + "truepng.exe " + ).c_str(), sInputFile, "", 0, 0);
        }
    }
}
