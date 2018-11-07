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
        private readonly bool _saveFailedOptimizedFile;

        private readonly string _fileOptimizerOptions;

        public FileOptimizerProcessor(string pathToFileOptimizer, string tempDirectory, bool shouldShowFileOptimizerWindow = false, int logLevel = 2, bool saveFailedOptimizedFile = false)
        {
            _pathToFileOptimizer = pathToFileOptimizer;
            _tempDirectory = tempDirectory;
            _shouldShowFileOptimizerWindow = shouldShowFileOptimizerWindow;
            _saveFailedOptimizedFile = saveFailedOptimizedFile;

            _fileOptimizerOptions = ConstantsAndConfig.GenerateOptimizerOptions(logLevel);

            Directory.CreateDirectory(tempDirectory);
        }

        public async Task<OptimizedFileResult> OptimizeFile(string fileToOptimize, string originDirectory)
        {
            var w = Stopwatch.StartNew();

            long originalFileSize = new FileInfo(fileToOptimize).Length;

            var tempFiles = new List<string>();

            var errors = new List<string>();

            try
            {
                Console.WriteLine($"=== Optimizing image: {fileToOptimize} ===");

                var fileName = Path.GetFileName(fileToOptimize);

                Console.WriteLine($"1: Ultraverbose: {fileToOptimize}");

                var tempFilePath = Path.Combine(_tempDirectory, RandomFileNameHelper.RandomizeFileName(fileName));
                Console.WriteLine($"2: Ultraverbose: {fileToOptimize}");
                tempFiles.Add(tempFilePath);
                Console.WriteLine($"3: Ultraverbose: {fileToOptimize}");

                await AsyncFileHelper.CopyFileAsync(fileToOptimize, tempFilePath, true);
                Console.WriteLine($"4: Ultraverbose: {fileToOptimize}");

                Orientation? jpegFileOrientation = null;
                Console.WriteLine($"5: Ultraverbose: {fileToOptimize}");
                bool shouldUseJpgWorkaround = FileTypeHelper.IsJpgFile(tempFilePath);
                Console.WriteLine($"6: Ultraverbose: {fileToOptimize}");
                if (shouldUseJpgWorkaround)
                {
                    Console.WriteLine($"7: Ultraverbose: {fileToOptimize}");
                    jpegFileOrientation = await ExifImageRotator.UnrotateImageAsync(tempFilePath);
                    Console.WriteLine($"8: Ultraverbose: {fileToOptimize}");
                }
                Console.WriteLine($"9: Ultraverbose: {fileToOptimize}");
                var processStartInfo = new ProcessStartInfo(_pathToFileOptimizer, $" {_fileOptimizerOptions} \"{tempFilePath}\"");
                Console.WriteLine($"10: Ultraverbose: {fileToOptimize}");
                if (!_shouldShowFileOptimizerWindow)
                {
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processStartInfo.UseShellExecute = true;
                    processStartInfo.CreateNoWindow = false;
                }
                Console.WriteLine($"11: Ultraverbose: {fileToOptimize}");

                var exitCode = await ProcessRunner.RunProcessAsync(processStartInfo);

                Console.WriteLine($"12: Ultraverbose: {fileToOptimize}");
                if (exitCode != 0)
                {
                    errors.Add($"Error when running FileOptimizer, Exit code: {exitCode}");
                }

                Console.WriteLine($"13: Ultraverbose: {fileToOptimize}");
                if (shouldUseJpgWorkaround)
                {
                    Console.WriteLine($"1: Ultraverbose: {fileToOptimize}");
                    await ExifImageRotator.RerotateImageAsync(tempFilePath, jpegFileOrientation);
                    Console.WriteLine($"1: Ultraverbose: {fileToOptimize}");
                }

                Console.WriteLine($"14: Ultraverbose: {fileToOptimize}");
                var imagesEqual = await ImageComparer.AreImagesEqualAsync(fileToOptimize, tempFilePath);

                Console.WriteLine($"15: Ultraverbose: {fileToOptimize}");
                if (!imagesEqual)
                {
                    errors.Add("Optimized image isn't equal to source image.");
                }

                Console.WriteLine($"16: Ultraverbose: {fileToOptimize}");

                var newSize = new FileInfo(tempFilePath).Length;
                if (newSize > originalFileSize)
                {
                    errors.Add($"Result image size is bigger then original. Original: {originalFileSize}, NewSize: {newSize}");
                }

                Console.WriteLine($"17: Ultraverbose: {fileToOptimize}");

                if (errors.Count == 0 && newSize < originalFileSize)
                {
                    await AsyncFileHelper.CopyFileAsync(tempFilePath, fileToOptimize, true);
                }
                else if (errors.Count != 0)
                {
                    if (_saveFailedOptimizedFile)
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
