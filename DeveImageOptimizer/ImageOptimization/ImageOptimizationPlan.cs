using DeveCoolLib.ProcessAsTask;
using DeveImageOptimizer.FileProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOptimization
{
    public class ImageOptimizationPlan
    {
        public DeveImageOptimizerConfiguration Configuration { get; }

        public ImageOptimizationPlan(DeveImageOptimizerConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<ImageOptimizationPlanResult> GoOptimize(string imagePath, ImageOptimizationLevel imageOptimizationLevel)
        {
            var ext = Path.GetExtension(imagePath).ToUpperInvariant();

            var steps = new List<ImageOptimizationStep>();

            var toolpath = Path.Join(Path.GetDirectoryName(Configuration.FileOptimizerPath), "Plugins64");

            switch (ext)
            {
                case var e when ConstantsFileExtensions.JPGExtensions.Contains(e.ToUpperInvariant()):
                    //Plugin: jhead (3/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jhead.exe -q -autorot -zt  "Z:\FileOptimizerTemp\FileOptimizer_Input_4294959198_2.jpg"
                    //Plugin: Leanify (4/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\leanify.exe -q --keep-exif --keep-icc-profile --jpeg-keep-all-metadata -i 30 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294960873_2.jpg"
                    //Plugin: jpegoptim (6/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegoptim.exe -o -q --all-progressive "Z:\FileOptimizerTemp\FileOptimizer_Input_8520_2.jpg"
                    //Plugin: jpegtran (7/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegtran.exe -progressive -optimize -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg" "Z:\FileOptimizerTemp\FileOptimizer_Output_4294960987_2.jpg"
                    //Plugin: mozjpegtran (8/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\mozjpegtran.exe -outfile "Z:\FileOptimizerTemp\FileOptimizer_Output_8530_2.jpg" -progressive -optimize -perfect -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg"
                    //Plugin: ECT (9/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\ECT.exe -quiet --allfilters -progressive -9 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294965741_2.jpg"

                    var tempImagePath = "tempimage.jpg";


                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jhead.exe"), $"-autorot -zt  \"{imagePath}\""));

                    var leanifyLevel = imageOptimizationLevel switch
                    {
                        ImageOptimizationLevel.SuperFast => 1,
                        ImageOptimizationLevel.Fast => 1,
                        ImageOptimizationLevel.Normal => 9,
                        ImageOptimizationLevel.Maximum => 30,
                        ImageOptimizationLevel.Placebo => 30,
                        _ => throw new NotSupportedException()
                    };
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "Leanify.exe"), $"--keep-exif --keep-icc-profile --jpeg-keep-all-metadata -i 30 \"{imagePath}\""));


                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegoptim.exe"), $"-o --all-progressive \"{imagePath}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegtran.exe"), $"-progressive -optimize -optimize -copy all \"{imagePath}\" \"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "mozjpegtran.exe"), $"-outfile \"{tempImagePath}\" -progressive -optimize -perfect -optimize -copy all \"{imagePath}\""));

                    var ectLevel = imageOptimizationLevel switch
                    {
                        ImageOptimizationLevel.SuperFast => 1,
                        ImageOptimizationLevel.Fast => 3,
                        ImageOptimizationLevel.Normal => 5,
                        ImageOptimizationLevel.Maximum => 9,
                        ImageOptimizationLevel.Placebo => 30060,
                        _ => throw new NotSupportedException()
                    };

                    var extraTagB = imageOptimizationLevel == ImageOptimizationLevel.Placebo ? "-b" : "";

                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "ECT.exe"), $"--allfilters{extraTagB} -progressive -{ectLevel} \"{imagePath}\""));

                    break;
                case var e when ConstantsFileExtensions.PNGExtensions.Contains(e.ToUpperInvariant()):
                    throw new NotImplementedException("This is not yet implemented");
                    break;
                case var e when ConstantsFileExtensions.BMPExtensions.Contains(e.ToUpperInvariant()):
                    throw new NotImplementedException("This is not yet implemented");
                    break;
                case var e when ConstantsFileExtensions.GIFExtensions.Contains(e.ToUpperInvariant()):
                    throw new NotImplementedException("This is not yet implemented");
                    break;
                default:
                    throw new NotImplementedException("This is not yet implemented");
            }
            bool success = true;

            var outputLog = new List<string>();
            var errorsLog = new List<string>();
            string imagePathCur = imagePath;
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                Console.WriteLine($"({i + 1}/{steps.Count}) Executing step {step.ToolExeFileName} for {Path.GetFileName(imagePath)}");

                outputLog.Add($"({i + 1}/{steps.Count}) {step.ToolExeFileName} {step.Arguments}");

                var fileSizeBefore = new FileInfo(imagePathCur).Length;
                var result = await step.Run(Configuration, imagePathCur);
                var fileSizeAfter = new FileInfo(result.OutputPath).Length;

                if (fileSizeAfter < fileSizeBefore)
                {
                    imagePathCur = result.OutputPath;
                }

                outputLog.AddRange(result.ProcessResults.StandardOutput);
                errorsLog.AddRange(result.ProcessResults.StandardError);

                if (result.ProcessResults.ExitCode != 0)
                {
                    Console.WriteLine($"Optimization failed for ({i + 1}/{steps.Count}) {step.ToolExeFileName} {step.Arguments}");
                    success = false;
                    break;
                }

                var warnIsBigger = fileSizeAfter > fileSizeBefore ? "(SKIPPED! After is larger then before)" : $"(Reduced by {fileSizeBefore - fileSizeAfter})";
                var resultString = $"Optimization successfull: {fileSizeBefore} => {fileSizeAfter} {warnIsBigger} (Elapsed: {Math.Round(result.ProcessResults.RunTime.TotalSeconds, 2) } seconds)";
                Console.WriteLine(resultString);
                outputLog.Add(resultString);
                outputLog.Add("");
            }

            return new ImageOptimizationPlanResult(success, imagePathCur, outputLog, errorsLog);
        }
    }
}
