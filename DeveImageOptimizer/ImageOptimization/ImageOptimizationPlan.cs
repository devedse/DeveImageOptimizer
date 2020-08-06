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

        public async Task<ImageOptimizationPlanResult> GoOptimize(string imagePath, ImageOptimizationLevel imageOptimizationLevel, List<string> tempFiles)
        {
            var ext = Path.GetExtension(imagePath).ToUpperInvariant();
            var steps = DeterminePlan(imageOptimizationLevel, ext);



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
                var result = await step.Run(Configuration, imagePathCur, tempFiles);
                var fileSizeAfter = new FileInfo(result.OutputPath).Length;

                if (fileSizeAfter < fileSizeBefore && fileSizeAfter > 0)
                {
                    imagePathCur = result.OutputPath;
                }

                outputLog.AddRange(result.ProcessResults.StandardOutput);
                errorsLog.AddRange(result.ProcessResults.StandardError);

                //if (result.ProcessResults.ExitCode != 0)
                //{
                //    PNGOUT fails if it can't optimize further. So we skip this check
                //    Console.WriteLine($"Optimization failed for ({i + 1}/{steps.Count}) {step.ToolExeFileName} {step.Arguments}");
                //    success = false;
                //    break;
                //}

                var warnIsBigger = fileSizeAfter > fileSizeBefore ? "(SKIPPED! After is larger then before)" : $"(Reduced by {fileSizeBefore - fileSizeAfter})";
                var resultString = $"Optimization successfull: {fileSizeBefore} => {fileSizeAfter} {warnIsBigger} (Elapsed: {Math.Round(result.ProcessResults.RunTime.TotalSeconds, 2) } seconds)";
                Console.WriteLine(resultString);
                outputLog.Add(resultString);
                outputLog.Add("");
            }

            return new ImageOptimizationPlanResult(success, imagePathCur, outputLog, errorsLog);
        }

        private List<ImageOptimizationStep> DeterminePlan(ImageOptimizationLevel imageOptimizationLevel, string ext)
        {

            var toolpath = Path.Join(Path.GetDirectoryName(Configuration.FileOptimizerPath), "Plugins64");
            var steps = new List<ImageOptimizationStep>();

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


                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jhead.exe"), $"-autorot -zt  \"{ImageOptimizationStep.InputFileToken}\""));

                    var leanifyLevel = imageOptimizationLevel switch
                    {
                        ImageOptimizationLevel.SuperFast => 1,
                        ImageOptimizationLevel.Fast => 1,
                        ImageOptimizationLevel.Normal => 9,
                        ImageOptimizationLevel.Maximum => 30,
                        ImageOptimizationLevel.Placebo => 30,
                        _ => throw new NotSupportedException()
                    };
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "Leanify.exe"), $"--keep-exif --keep-icc-profile --jpeg-keep-all-metadata -i 30 \"{ImageOptimizationStep.InputFileToken}\""));


                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegoptim.exe"), $"-o --all-progressive \"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegtran.exe"), $"-progressive -optimize -optimize -copy all \"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "mozjpegtran.exe"), $"-outfile \"{tempImagePath}\" -progressive -optimize -perfect -optimize -copy all \"{ImageOptimizationStep.InputFileToken}\""));

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

                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "ECT.exe"), $"--allfilters{extraTagB} -progressive -{ectLevel} \"{ImageOptimizationStep.InputFileToken}\""));

                    break;
                case var e when ConstantsFileExtensions.PNGExtensions.Contains(e.ToUpperInvariant()):
                    //Plugin: PngOptimizer (3/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\PngOptimizer.exe -KeepPhysicalPixelDimensions -file:"Z:\FileOptimizerTemp\FileOptimizer_Input_4294962652_1.png"
                    //Plugin: TruePNG (4/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\truepng.exe -o4 -md keep all /i0 /nc /tz /quiet /y /out "Z:\FileOptimizerTemp\FileOptimizer_Output_4294960559_1.png" "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\1.png"
                    //Plugin: PNGOut (5/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\pngout.exe /q /y /r /d0 /mincodes0 /k1 /s0 "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\1.png" "Z:\FileOptimizerTemp\FileOptimizer_Output_4294965414_1.png"
                    //Plugin: OptiPNG (7/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\optipng.exe -zw32k -quiet -o6 "Z:\FileOptimizerTemp\FileOptimizer_Input_8625_1.png"
                    //Plugin: pngwolf (9/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\pngwolf.exe --out-deflate=zopfli,iter=30 --in="C:\Users\Davy\Desktop\TestImages\FileOptimizer1\1.png" --out="Z:\FileOptimizerTemp\FileOptimizer_Output_1440_1.png"
                    //Plugin: pngrewrite (10/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\pngrewrite.exe "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\1.png" "Z:\FileOptimizerTemp\FileOptimizer_Output_3539_1.png"
                    //Plugin: ECT (12/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\ECT.exe -quiet --allfilters -9 "Z:\FileOptimizerTemp\FileOptimizer_Input_7162_1.png"
                    //Plugin: DeflOpt (14/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\deflopt.exe /a /b /s /k "Z:\FileOptimizerTemp\FileOptimizer_Input_4294964645_1.png"
                    //Plugin: defluff (15/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\defluff.bat "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\1.png" "Z:\FileOptimizerTemp\FileOptimizer_Output_8276_1.png"
                    //Plugin: DeflOpt (16/16)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\deflopt.exe /a /b /s /k "Z:\FileOptimizerTemp\FileOptimizer_Input_4983_1.png"

                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "PngOptimizer.exe"), $"-KeepPhysicalPixelDimensions -file:\"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "truepng.exe"), $"-o4 -md keep all /i0 /nc /tz /y /out \"{ImageOptimizationStep.OutputFileToken}\" \"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngout.exe"), $"/y /r /d0 /mincodes0 /k1 /s0 \"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "optipng.exe"), $"-zw32k -o6 \"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngwolf.exe"), $"--out-deflate=zopfli,iter=30 --in=\"{ImageOptimizationStep.InputFileToken}\" --out=\"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngrewrite.exe"), $"\"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "ECT.exe"), $"--allfilters -9 \"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "deflopt.exe"), $"/a /b /k \"{ImageOptimizationStep.InputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "defluff.bat"), $"\"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\""));
                    steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "deflopt.exe"), $"/a /b /k \"{ImageOptimizationStep.InputFileToken}\""));


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

            return steps;
        }
    }
}
