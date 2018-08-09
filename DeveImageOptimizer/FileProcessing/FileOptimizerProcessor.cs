using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using ExifLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeveImageOptimizer.FileProcessing
{
    public class FileOptimizerProcessor
    {
        private readonly string _pathToFileOptimizer;
        private readonly string _tempDirectory;
        private readonly bool _shouldShowFileOptimizerWindow;

        public FileOptimizerProcessor(string pathToFileOptimizer, string tempDirectory, bool shouldShowFileOptimizerWindow)
        {
            _pathToFileOptimizer = pathToFileOptimizer;
            _tempDirectory = tempDirectory;
            _shouldShowFileOptimizerWindow = shouldShowFileOptimizerWindow;
            Directory.CreateDirectory(tempDirectory);
        }

        public async Task<OptimizedFileResult> OptimizeFile(string fileToOptimize, string originDirectory, bool saveFailedOptimizedFile = false)
        {
            var w = Stopwatch.StartNew();

            long originalFileSize = new FileInfo(fileToOptimize).Length;

            var tempFiles = new List<string>();

            var errors = new List<string>();

            try
            {
                Console.WriteLine($"=== Optimizing image: {fileToOptimize} ===");

                var fileName = Path.GetFileName(fileToOptimize);
                var tempFilePath = Path.Combine(_tempDirectory, RandomFileNameHelper.RandomizeFileName(fileName));
                tempFiles.Add(tempFilePath);

                await AsyncFileHelper.CopyFileAsync(fileToOptimize, tempFilePath, true);

                Orientation? jpegFileOrientation = null;
                bool shouldUseJpgWorkaround = FileTypeHelper.IsJpgFile(tempFilePath);
                if (shouldUseJpgWorkaround)
                {
                    jpegFileOrientation = await ExifImageRotator.UnrotateImageAsync(tempFilePath);
                }

                var processStartInfo = new ProcessStartInfo(_pathToFileOptimizer, $" {Constants.OptimizerOptions} \"{tempFilePath}\"");

                if (!_shouldShowFileOptimizerWindow)
                {
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processStartInfo.UseShellExecute = true;
                    processStartInfo.CreateNoWindow = false;
                }

                var exitCode = await ProcessRunner.RunProcessAsync(processStartInfo);

                if (exitCode != 0)
                {
                    errors.Add($"Error when running FileOptimizer, Exit code: {exitCode}");
                }

                if (shouldUseJpgWorkaround)
                {
                    await ExifImageRotator.RerotateImageAsync(tempFilePath, jpegFileOrientation);
                }

                var imagesEqual = await ImageComparer.AreImagesEqualAsync(fileToOptimize, tempFilePath);

                if (!imagesEqual)
                {
                    errors.Add("Optimized image isn't equal to source image.");
                }

                var newSize = new FileInfo(tempFilePath).Length;
                if (newSize > originalFileSize)
                {
                    errors.Add($"Result image size is bigger then original. Original: {originalFileSize}, NewSize: {newSize}");
                }

                if (errors.Count == 0 && newSize < originalFileSize)
                {
                    await AsyncFileHelper.CopyFileAsync(tempFilePath, fileToOptimize, true);
                }
                else if (errors.Count != 0)
                {
                    if (saveFailedOptimizedFile)
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileToOptimize);
                        var fileExtension = Path.GetExtension(fileToOptimize);
                        var newFileName = $"{fileNameWithoutExtension}_FAILED{fileExtension}";

                        var directoryOfFileToOptimize = Path.GetDirectoryName(fileToOptimize);
                        var newFilePath = Path.Combine(directoryOfFileToOptimize, newFileName);

                        //Write a file as Blah_FAILED.png
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
            long optimizedFileSize = new FileInfo(fileToOptimize).Length;

            var optimizedFileResult = new OptimizedFileResult(fileToOptimize, RelativePathFinderHelper.GetRelativePath(originDirectory, fileToOptimize), errors.Count == 0 ? OptimizationResult.Success : OptimizationResult.Failed, originalFileSize, optimizedFileSize, w.Elapsed, errors);

            if (errors.Any())
            {
                Console.WriteLine("Errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"\tError: {error}");
                }
            }
            Console.WriteLine($"Image optimization completed in {w.Elapsed}. Result: {optimizedFileResult.OptimizationResult}. Bytes saved: {ValuesToStringHelper.BytesToString(optimizedFileResult.OriginalSize - optimizedFileResult.OptimizedSize)}");

            return optimizedFileResult;
        }
    }
}
