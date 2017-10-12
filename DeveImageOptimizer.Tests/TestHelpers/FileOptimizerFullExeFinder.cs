using System.Collections.Generic;
using System.IO;
using Xunit;

namespace DeveImageOptimizer.Tests.TestHelpers
{
    public static class FileOptimizerFullExeFinder
    {
        public static string GetFileOptimizerPathOrThrowSkipTestException()
        {
            var possiblePaths = new List<string>();

            possiblePaths.Add(@"C:\Program Files\FileOptimizer\FileOptimizer64.exe");

            //Maybe this can be improved, but just to test I'll have it like this for now. (It's just for unit testing anyway)
            var extractedPathForAppVeyor = Path.GetFullPath(@"..\..\..\..\..\DeveImageOptimizer\Scripts\FileOptimizer\FileOptimizer64.exe");
            possiblePaths.Add(extractedPathForAppVeyor);

            var extractedPathForAppVeyor2 = Path.GetFullPath(@"..\..\..\..\DeveImageOptimizer\Scripts\FileOptimizer\FileOptimizer64.exe");
            possiblePaths.Add(extractedPathForAppVeyor2);

            foreach (var possiblePath in possiblePaths)
            {
                if (File.Exists(possiblePath))
                {
                    return possiblePath;
                }
            }

            throw new SkipException($"FileOptimizerFull exe file can't be found. Expected locations: " + string.Join(' ', possiblePaths));
        }
    }
}
