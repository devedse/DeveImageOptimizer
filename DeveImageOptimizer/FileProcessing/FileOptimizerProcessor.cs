using DeveCoolLib.Conversion;
using DeveImageOptimizer.Exceptions;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State;
using ExifLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DeveImageOptimizer.FileProcessing
{
    public class FileOptimizerProcessor
    {
        public DeveImageOptimizerConfiguration Configuration { get; }

        public FileOptimizerProcessor(DeveImageOptimizerConfiguration configuration)
        {
            Configuration = configuration;

            Directory.CreateDirectory(Configuration.TempDirectory);
            Directory.CreateDirectory(Configuration.FailedFilesDirectory);
        }

        public async Task OptimizeFile(OptimizableFile file)
        {
            if (!File.Exists(Configuration.FileOptimizerPath))
            {
                throw new FileOptimizerNotFoundException(Configuration.FileOptimizerPath);
            }

            var ext = Path.GetExtension(file.Path).ToUpperInvariant();
            if (ConstantsFileExtensions.ShouldSkipBasedOnConfiguration(Configuration, ext))
            {
                Console.WriteLine($"=== Skipping image: {file.Path} due to extension '{ext}' being disabled in configuration ===");
                file.SetSkipped();
                return;
            }

            var w = Stopwatch.StartNew();

            var tempFiles = new List<string>();

            var errors = new List<string>();

            try
            {
                Console.WriteLine($"=== Optimizing image: {file.Path} ===");

                var fileName = Path.GetFileName(file.Path);
                var tempFilePath = Path.Combine(Configuration.TempDirectory, RandomFileNameHelper.RandomizeFileName(fileName));
                tempFiles.Add(tempFilePath);

                var originalCreationTime = File.GetCreationTimeUtc(file.Path);
                var originalLastModifiedTime = File.GetLastWriteTimeUtc(file.Path);
                await AsyncFileHelper.CopyFileAsync(file.Path, tempFilePath, true);

                Orientation? jpegFileOrientation = null;
                bool shouldUseJpgWorkaround = FileTypeHelper.IsJpgFile(tempFilePath);
                if (shouldUseJpgWorkaround)
                {
                    //From what I've found in my latest tests is that this is apparently not needed anymore
                    //for the latest version of FileOptimizer or directly calling the plugins
                    //For now I've just left it here as it doesn't hurt either.
                    jpegFileOrientation = await ExifImageRotator.UnrotateImageAsync(tempFilePath);
                }

                if (Configuration.CallOptimizationToolsDirectlyInsteadOfThroughFileOptimizer)
                {
                    var optimizationPlan = new ImageOptimizationPlan(Configuration);
                    var result = await optimizationPlan.GoOptimize(tempFilePath, Configuration.ImageOptimizationLevel, tempFiles);

                    tempFilePath = result.OutputPath;

                    if (!result.Success)
                    {
                        errors.Add($"Error when running NewDeveImageOptimization");
                        errors.AddRange(result.ErrorsLog);
                    }

                    //TODO: Do something with the logging
                }
                else
                {
                    var args = ConstantsAndConfig.GenerateOptimizerOptions(Configuration.LogLevel, Configuration.ImageOptimizationLevel);

                    //This next line should disable showing the window, but apparently it doesn't work yet as of version 13.50.2431
                    //if (!_shouldShowFileOptimizerWindow)
                    //{
                    //    args = $"/NoWindow {args}";
                    //}
                    var processStartInfo = new ProcessStartInfo(Configuration.FileOptimizerPath, $" {args} \"{tempFilePath}\"");
                    if (Configuration.HideOptimizerWindow)
                    {
                        processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        processStartInfo.UseShellExecute = true;
                        processStartInfo.CreateNoWindow = false;
                    }

                    var processResult = await ProcessRunnerOld.RunProcessAsync(processStartInfo);

                    if (processResult != 0)
                    {
                        errors.Add($"Error when running FileOptimizer, Exit code: {processResult}");
                    }
                }







                if (shouldUseJpgWorkaround)
                {
                    await ExifImageRotator.RerotateImageAsync(tempFilePath, jpegFileOrientation);
                }

                var imagesEqual = await ImageComparer.AreImagesEqualAsync(file.Path, tempFilePath);

                if (!imagesEqual)
                {
                    errors.Add("Optimized image isn't equal to source image.");
                }

                var newSize = new FileInfo(tempFilePath).Length;
                if (newSize > file.OriginalSize)
                {
                    errors.Add($"Result image size is bigger then original. Original: {file.OriginalSize}, NewSize: {newSize}");
                }

                if (errors.Count == 0 && newSize < file.OriginalSize)
                {

                    await AsyncFileHelper.CopyFileAsync(tempFilePath, file.Path, true);
                    if (Configuration.KeepFileAttributes)
                    {
                        File.SetCreationTimeUtc(file.Path, originalCreationTime);
                        File.SetLastWriteTimeUtc(file.Path, originalLastModifiedTime);
                    }
                }
                else if (errors.Count != 0)
                {
                    if (Configuration.SaveFailedFiles)
                    {
                        var newFilePath = Path.Combine(Configuration.FailedFilesDirectory, fileName);

                        //Write a file as FailedFiles\Blah.png
                        await AsyncFileHelper.CopyFileAsync(tempFilePath, newFilePath, true);
                    }
                }
                else
                {
                    Console.WriteLine("Image could not be further optimized.");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Exception happened when optimizing or comparing the image:{Environment.NewLine}{ex}");
            }
            finally
            {
                foreach (var tempFile in tempFiles)
                {
                    FileHelperMethods.SafeDeleteTempFile(tempFile);
                }
            }

            w.Stop();
            //The fileToOptimize has been overwritten by the optimized file, so this is the optimized file size.
            long optimizedFileSize = new FileInfo(file.Path).Length;

            if (errors.Count == 0)
            {
                file.SetSuccess(optimizedFileSize, w.Elapsed, Configuration.ImageOptimizationLevel);
            }
            else
            {
                Console.WriteLine("Errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"\tError: {error}");
                }
                file.SetFailed(w.Elapsed, errors);
            }
            Console.WriteLine($"Image optimization completed in {w.Elapsed}. Result: {file.OptimizationResult}. Bytes saved: {ValuesToStringHelper.BytesToString(file.OriginalSize - file.OptimizedSize)}");
        }
    }
}
