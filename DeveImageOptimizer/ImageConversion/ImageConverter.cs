using System;

namespace DeveImageOptimizer.ImageConversion
{
    public static class ImageConverter
    {
        public static string ConvertJpgToPngWithAutoRotate(string inputPath)
        {
            throw new InvalidOperationException("Using VIPS has been removed from DeveImageOptimizer because ImageSharp works great now and no bugs have to be worked around");
            //string pathVips = DeveLibVipsNuget.LibVipsManager.ExtractAndGetVipsExeFile();

            //var imageName = Path.GetFileNameWithoutExtension(inputPath);
            //var outPath = Path.Combine(FolderHelperMethods.TempDirectory, RandomFileNameHelper.RandomizeFileName(imageName, "png"));
            //string args = $"COPY \"{inputPath}\"[autorotate] \"{outPath}\"";

            //var processStartInfo = new ProcessStartInfo(pathVips, args)
            //{
            //    WindowStyle = ProcessWindowStyle.Hidden,
            //    UseShellExecute = true,
            //    CreateNoWindow = false
            //};

            //using (var proc = Process.Start(processStartInfo))
            //{
            //    proc.WaitForExit();
            //    return outPath;
            //}
        }
    }
}
