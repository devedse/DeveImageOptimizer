using DeveImageOptimizer.Helpers;
using System.Diagnostics;
using System.IO;

namespace DeveImageOptimizer.ImageConversion
{
    public static class ImageConverter
    {
        public static string ConvertJpgToPngIrfanView(string inputPath)
        {
            //i_view32.exe "C:\drawing*.png" /convert="C:\Converted\$N.jpg"

            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value);

            var userProfileDir = System.Environment.GetEnvironmentVariable("USERPROFILE");
            //var userProfileDir2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //string pathVips = Path.Combine(userProfileDir, Constants.VipsDir, "vips.exe");
            string pathIrfanView = @"C:\Program Files (x86)\IrfanView\i_view32.exe";

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"\"{inputPath}\" /convert=\"{outPath}\"";

            var psi = new ProcessStartInfo(pathIrfanView, args);

            var proc = Process.Start(psi);

            proc.WaitForExit();

            return outPath;
        }


        public static string ConvertJpgToPng(string inputPath)
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value);

            var userProfileDir = System.Environment.GetEnvironmentVariable("USERPROFILE");
            //var userProfileDir2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathVips = Path.Combine(userProfileDir, Constants.VipsDir, "vips.exe");

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"COPY \"{inputPath}\" \"{outPath}\"";

            var psi = new ProcessStartInfo(pathVips, args);

            var proc = Process.Start(psi);

            proc.WaitForExit();

            return outPath;
        }

        public static string ConvertJpgToPngWithAutoRotate(string inputPath)
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value);

            var userProfileDir = System.Environment.GetEnvironmentVariable("USERPROFILE");
            //var userProfileDir2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathVips = Path.Combine(userProfileDir, Constants.VipsDir, "vips.exe");

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"COPY \"{inputPath}\"[autorotate] \"{outPath}\"";

            var psi = new ProcessStartInfo(pathVips, args);

            var proc = Process.Start(psi);

            proc.WaitForExit();

            return outPath;
        }

        public static string ConvertJpgToPngWithRotation(string inputPath, string rotation)
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value);

            var userProfileDir = System.Environment.GetEnvironmentVariable("USERPROFILE");
            //var userProfileDir2 = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathVips = Path.Combine(userProfileDir, Constants.VipsDir, "vips.exe");

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"ROT \"{inputPath}\" \"{outPath}\" d{rotation}";

            var psi = new ProcessStartInfo(pathVips, args);

            var proc = Process.Start(psi);

            proc.WaitForExit();

            return outPath;
        }
    }
}
