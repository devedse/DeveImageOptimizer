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

            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);

            string pathIrfanView = @"C:\Program Files (x86)\IrfanView\i_view32.exe";

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"\"{inputPath}\" /convert=\"{outPath}\"";

            var processStartInfo = new ProcessStartInfo(pathIrfanView, args);

            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = true;
            processStartInfo.CreateNoWindow = false;

            using (var proc = Process.Start(processStartInfo))
            {
                proc.WaitForExit();
                return outPath;
            }
        }

        public static string ConvertJpgToPngWithAutoRotate(string inputPath)
        {
            Directory.CreateDirectory(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value);

            string pathVips = DeveLibVipsNuget.LibVipsManager.ExtractAndGetVipsExeFile();

            var imageName = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyTempDirectory.Value, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            string args = $"COPY \"{inputPath}\"[autorotate] \"{outPath}\"";

            var processStartInfo = new ProcessStartInfo(pathVips, args);

            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = true;
            processStartInfo.CreateNoWindow = false;

            using (var proc = Process.Start(processStartInfo))
            {
                proc.WaitForExit();
                return outPath;
            }
        }
    }
}
