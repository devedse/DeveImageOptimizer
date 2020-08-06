using DeveCoolLib.ProcessAsTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationPlan
    {
        public ImageOptimizationPlan()
        {

        }

        public async Task<bool> GoOptimize(string imagePath)
        {
            var ext = Path.GetExtension(imagePath).ToUpperInvariant();

            switch (ext)
            {
                case ".JPG":
                case ".JPEG":

                    //Plugin: jhead (3/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jhead.exe -q -autorot -zt  "Z:\FileOptimizerTemp\FileOptimizer_Input_4294959198_2.jpg"
                    //Plugin: Leanify (4/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\leanify.exe -q --keep-exif --keep-icc-profile --jpeg-keep-all-metadata -i 30 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294960873_2.jpg"
                    //Plugin: jpegoptim (6/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegoptim.exe -o -q --all-progressive "Z:\FileOptimizerTemp\FileOptimizer_Input_8520_2.jpg"
                    //Plugin: jpegtran (7/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegtran.exe -progressive -optimize -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg" "Z:\FileOptimizerTemp\FileOptimizer_Output_4294960987_2.jpg"
                    //Plugin: mozjpegtran (8/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\mozjpegtran.exe -outfile "Z:\FileOptimizerTemp\FileOptimizer_Output_8530_2.jpg" -progressive -optimize -perfect -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg"
                    //Plugin: ECT (9/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\ECT.exe -quiet --allfilters -progressive -9 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294965741_2.jpg"


                    var tempImagePath = "tempimage.jpg";


                    var res1 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\jhead.exe", $"-autorot -zt  \"{imagePath}\"");
                    var res2 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\Leanify.exe", $"--keep-exif --keep-icc-profile --jpeg-keep-all-metadata -i 30 \"{imagePath}\"");
                    var res3 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\jpegoptim.exe", $"-o --all-progressive \"{imagePath}\"");
                    var res4 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\jpegtran.exe", $"-progressive -optimize -optimize -copy all \"{imagePath}\" \"{tempImagePath}\"");
                    var res5 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\mozjpegtran.exe", $"-outfile \"{tempImagePath}\" -progressive -optimize -perfect -optimize -copy all \"{imagePath}\"");
                    var res6 = await ProcessRunner.RunAsyncAndLogToConsole(@"C:\Program Files\FileOptimizer\Plugins64\ECT.exe", $"--allfilters -progressive -9 \"{imagePath}\"");

                    return true;
                case ".GIF":
                case ".BMP":
                case ".PNG":
                default:
                    throw new NotImplementedException("This is not yet implemented");
            }


        }
    }
}
