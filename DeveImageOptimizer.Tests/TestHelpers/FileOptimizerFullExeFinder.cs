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

            var extractedPathForAppVeyor = Path.GetFullPath(@"..\..\..\..\..\DeveImageOptimizer\Scripts\FileOptimizer\FileOptimizer64.exe");
            possiblePaths.Add(extractedPathForAppVeyor);

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
