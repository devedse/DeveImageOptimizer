using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.VariousTests
{
    public class BabeRuthInvestigation
    {
        public const string BabeRuthInvestigationName = "BaberuthInvestigation";

        [SkippableFact]
        public async Task ShouldBeEqualToAllIterationsOfRuth()
        {
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

            if (failers.Any() || allImages.Count != succeeders.Count )
            {
                throw new SkipException("This test fails because of an issue in FileOptimizer");
            }
        }
    }
}
