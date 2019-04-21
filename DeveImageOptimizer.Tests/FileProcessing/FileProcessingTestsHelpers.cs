using DeveImageOptimizer.Helpers;
using System.IO;

namespace DeveImageOptimizer.Tests.FileProcessing
{
    public static class FileProcessingTestsHelpers
    {
        public static string PrepareTestOptimizeDir(string dirToOptimize, string fileNameFileProcessedStateRememberer, string fileNameDirProcessedStateRememberer, string testName)
        {
            var tempfortestdir = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, "TempForTest");
            var sourceSampleDirToOptimize = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, "TestImages", dirToOptimize);
            var sampleDirToOptimize = Path.Combine(tempfortestdir, testName);

            if (Directory.Exists(sampleDirToOptimize))
            {
                Directory.Delete(sampleDirToOptimize, true);
            }
            Directory.CreateDirectory(sampleDirToOptimize);
            foreach (var file in Directory.GetFiles(sourceSampleDirToOptimize))
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(sampleDirToOptimize, fileName);
                File.Copy(file, destPath);
            }

            //Delete optimized file storage
            var filePathOfRemembererStorage = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, fileNameFileProcessedStateRememberer);
            File.Delete(filePathOfRemembererStorage);
            var filePathOfDirRemembererStorage = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, fileNameDirProcessedStateRememberer);
            File.Delete(filePathOfDirRemembererStorage);
            return sampleDirToOptimize;
        }
    }
}
