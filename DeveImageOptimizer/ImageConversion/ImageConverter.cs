using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DeveImageOptimizer.ImageConversion
{
    public static class ImageConverter
    {
        public static string ConvertJpgToPng(string inputPath)
        {
            Directory.CreateDirectory(FolderHelperMethods.TempDirectoryForTests.Value);

            var userProfileDir = System.Environment.GetEnvironmentVariable("USERPROFILE");
            //var userProfileDir2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathVips = Path.Combine(userProfileDir, Constants.VipsDir, "vips.exe");

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.TempDirectoryForTests.Value, $"{imageName}.png");
            string args = $"COPY \"{inputPath}\" \"{outPath}\"";

            var psi = new ProcessStartInfo(pathVips, args);

            var proc = Process.Start(psi);

            proc.WaitForExit();

            return outPath;
        }
    }
}
