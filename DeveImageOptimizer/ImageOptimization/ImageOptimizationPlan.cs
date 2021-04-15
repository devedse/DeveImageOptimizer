using DeveCoolLib.ProcessAsTask;
using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
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
        public static object LOCKFORDEFLUFFFILECREATION = new object();
        public static bool DEFLUFFFILECREATED = false;

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
                var outFile = new FileInfo(result.OutputPath);

                if (outFile.Exists && outFile.Length < fileSizeBefore && outFile.Length > 0)
                {
                    imagePathCur = result.OutputPath;
                }

                var fileSizeAfter = outFile.Exists ? outFile.Length : fileSizeBefore;

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
                    {
                        //Plugin: jhead (3/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jhead.exe -q -autorot -zt  "Z:\FileOptimizerTemp\FileOptimizer_Input_4294959198_2.jpg"
                        //Plugin: Leanify (4/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\leanify.exe -q -p --keep-exif --keep-icc --jpeg-keep-all -i 30 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294960873_2.jpg"
                        //Plugin: jpegoptim (6/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegoptim.exe -o -q --all-progressive "Z:\FileOptimizerTemp\FileOptimizer_Input_8520_2.jpg"
                        //Plugin: jpegtran (7/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\jpegtran.exe -progressive -optimize -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg" "Z:\FileOptimizerTemp\FileOptimizer_Output_4294960987_2.jpg"
                        //Plugin: mozjpegtran (8/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\mozjpegtran.exe -outfile "Z:\FileOptimizerTemp\FileOptimizer_Output_8530_2.jpg" -progressive -optimize -perfect -optimize -copy all "C:\Users\Davy\Desktop\TestImages\FileOptimizer1\2.jpg"
                        //Plugin: ECT (9/10)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\ECT.exe -quiet --allfilters -progressive -9 "Z:\FileOptimizerTemp\FileOptimizer_Input_4294965741_2.jpg"

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jhead.exe"), $"-autorot -zt \"{ImageOptimizationStep.InputFileToken}\"", false));

                        var leanifyLevel = imageOptimizationLevel switch
                        {
                            ImageOptimizationLevel.SuperFast => 1,
                            ImageOptimizationLevel.Fast => 1,
                            ImageOptimizationLevel.Normal => 9,
                            ImageOptimizationLevel.Maximum => 30,
                            ImageOptimizationLevel.Placebo => 30,
                            _ => throw new NotSupportedException()
                        };
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "Leanify.exe"), $"--keep-exif --keep-icc --jpeg-keep-all -i {leanifyLevel} \"{ImageOptimizationStep.InputFileToken}\"", true));


                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegoptim.exe"), $"-o --all-progressive \"{ImageOptimizationStep.InputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "jpegtran.exe"), $"-progressive -optimize -optimize -copy all \"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "mozjpegtran.exe"), $"-outfile \"{ImageOptimizationStep.OutputFileToken}\" -progressive -optimize -perfect -optimize -copy all \"{ImageOptimizationStep.InputFileToken}\"", false));

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

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "ECT.exe"), $"--allfilters{extraTagB} -progressive -{ectLevel} \"{ImageOptimizationStep.InputFileToken}\"", false));
                    }
                    break;
                case var e when ConstantsFileExtensions.PNGExtensions.Contains(e.ToUpperInvariant()):
                    {
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

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "PngOptimizer.exe"), $"-KeepPhysicalPixelDimensions -file:\"{ImageOptimizationStep.InputFileToken}\"", false));

                        var truePngLevel = imageOptimizationLevel switch
                        {
                            ImageOptimizationLevel.SuperFast => 1,
                            ImageOptimizationLevel.Fast => 2,
                            ImageOptimizationLevel.Normal => 3,
                            ImageOptimizationLevel.Maximum => 4,
                            ImageOptimizationLevel.Placebo => 4,
                            _ => throw new NotSupportedException()
                        };
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "truepng.exe"), $"-o{truePngLevel} - md keep all /i0 /nc /tz /y /out \"{ImageOptimizationStep.OutputFileToken}\" \"{ImageOptimizationStep.InputFileToken}\"", true));

                        var pngOutLevel = imageOptimizationLevel switch
                        {
                            ImageOptimizationLevel.SuperFast => 3,
                            ImageOptimizationLevel.Fast => 2,
                            ImageOptimizationLevel.Normal => 1,
                            ImageOptimizationLevel.Maximum => 0,
                            ImageOptimizationLevel.Placebo => 0,
                            _ => throw new NotSupportedException()
                        };

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngout.exe"), $"/y /r /d{pngOutLevel} /mincodes0 /k1 /s0 \"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\"", true));

                        var optiPngLevel = imageOptimizationLevel switch
                        {
                            ImageOptimizationLevel.SuperFast => 1,
                            ImageOptimizationLevel.Fast => 3,
                            ImageOptimizationLevel.Normal => 4,
                            ImageOptimizationLevel.Maximum => 6,
                            ImageOptimizationLevel.Placebo => 7,
                            _ => throw new NotSupportedException()
                        };

                        var extraOptiPngFlags = "";
                        if (imageOptimizationLevel == ImageOptimizationLevel.Placebo)
                        {
                            extraOptiPngFlags = "-zm1-9 ";
                        }

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "optipng.exe"), $"-zw32k -o{optiPngLevel} {extraOptiPngFlags}\"{ImageOptimizationStep.InputFileToken}\"", false));

                        var pngWolfIterations = imageOptimizationLevel switch
                        {
                            ImageOptimizationLevel.SuperFast => 1,
                            ImageOptimizationLevel.Fast => 3,
                            ImageOptimizationLevel.Normal => 9,
                            ImageOptimizationLevel.Maximum => 30,
                            ImageOptimizationLevel.Placebo => 30,
                            _ => throw new NotSupportedException()
                        };

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngwolf.exe"), $"--out-deflate=zopfli,iter={pngWolfIterations} --in=\"{ImageOptimizationStep.InputFileToken}\" --out=\"{ImageOptimizationStep.OutputFileToken}\"", true));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "pngrewrite.exe"), $"\"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\"", false));

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

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "ECT.exe"), $"--allfilters{extraTagB} -{ectLevel} \"{ImageOptimizationStep.InputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "deflopt.exe"), $"/a /b /k \"{ImageOptimizationStep.InputFileToken}\"", true));

                        //This hack is required because defluff.exe expects piped images. Fileoptimizerfull does this by calling a .bat file, but this bat file uses a relative path. So we fix this to be an absolute path.
                        var expectedDefluff2batpath = Path.Join(FolderHelperMethods.TempDirectory, "defluff2.bat");
                        if (!DEFLUFFFILECREATED)
                        {
                            lock (LOCKFORDEFLUFFFILECREATION)
                            {
                                if (!DEFLUFFFILECREATED)
                                {
                                    File.WriteAllText(expectedDefluff2batpath, $"@\"{Path.Join(toolpath, "defluff.exe")}\" <%1 >%2");
                                    DEFLUFFFILECREATED = true;
                                }
                            }
                        }


                        steps.Add(new ImageOptimizationStep(expectedDefluff2batpath, $"\"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "deflopt.exe"), $"/a /b /k \"{ImageOptimizationStep.InputFileToken}\"", true));

                    }
                    break;
                case var e when ConstantsFileExtensions.BMPExtensions.Contains(e.ToUpperInvariant()):
                    {
                        //Plugin: ImageMagick (1/2)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\magick.exe convert "C:\Users\Davy\AppData\Local\Temp\DeveImageOptimizerTemp\TestImageBMP_xnvdctl3.bmp" -quiet -compress RLE "Z:\FileOptimizerTemp\FileOptimizer_Output_8722_TestImageBMP_xnvdctl3.bmp"
                        //Plugin: ImageWorsener (2/2)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\imagew.exe -noresize -zipcmprlevel 9 -outfmt bmp -compress "rle" "C:\Users\Davy\AppData\Local\Temp\DeveImageOptimizerTemp\TestImageBMP_xnvdctl3.bmp" "Z:\FileOptimizerTemp\FileOptimizer_Output_1885_TestImageBMP_xnvdctl3.bmp"

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "magick.exe"), $"convert \"{ImageOptimizationStep.InputFileToken}\" -compress RLE \"{ImageOptimizationStep.OutputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "imagew.exe"), $"-noresize -zipcmprlevel 9 -outfmt bmp -compress \"rle\" \"{ImageOptimizationStep.InputFileToken}\" \"{ImageOptimizationStep.OutputFileToken}\"", false));
                    }
                    break;
                case var e when ConstantsFileExtensions.GIFExtensions.Contains(e.ToUpperInvariant()):
                    {
                        //Plugin: ImageMagick (1/2)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\magick.exe convert "C:\Users\Davy\AppData\Local\Temp\DeveImageOptimizerTemp\TestImageGIF_gykbljhq.gif" -quiet -set dispose background -layers optimize -compress -loop 0 LZW "Z:\FileOptimizerTemp\FileOptimizer_Output_4294964962_TestImageGIF_gykbljhq.gif"
                        //Plugin: gifsicle (2/2)	Commandline: C:\PROGRA~1\FILEOP~1\PLUGIN~1\gifsicle.exe -w -j --no-conserve-memory -o "Z:\FileOptimizerTemp\FileOptimizer_Output_4294966173_TestImageGIF_gykbljhq.gif" -O3 "C:\Users\Davy\AppData\Local\Temp\DeveImageOptimizerTemp\TestImageGIF_gykbljhq.gif"

                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "magick.exe"), $"convert \"{ImageOptimizationStep.InputFileToken}\" -set dispose background -layers optimize -compress -loop 0 LZW \"{ImageOptimizationStep.OutputFileToken}\"", false));
                        steps.Add(new ImageOptimizationStep(Path.Join(toolpath, "gifsicle.exe"), $"-w -j --no-conserve-memory -o \"{ImageOptimizationStep.OutputFileToken}\" -O3 \"{ImageOptimizationStep.InputFileToken}\"", false));
                    }
                    break;
                default:
                    throw new NotImplementedException("This is not yet implemented");
            }

            return steps;
        }
    }
}
