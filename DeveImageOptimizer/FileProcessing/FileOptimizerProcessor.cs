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
        private readonly string _failedFilesDirectory;
        private readonly bool _shouldShowFileOptimizerWindow;
        private readonly bool _saveFailedOptimizedFile;

        private readonly string _fileOptimizerOptions;

        public FileOptimizerProcessor(string pathToFileOptimizer, string tempDirectory = null, string failedFilesDirectory = null, bool shouldShowFileOptimizerWindow = false, int logLevel = 2, bool saveFailedOptimizedFile = false)
        {
            _pathToFileOptimizer = pathToFileOptimizer;
            _tempDirectory = tempDirectory;
            _failedFilesDirectory = failedFilesDirectory;
            _shouldShowFileOptimizerWindow = shouldShowFileOptimizerWindow;
            _saveFailedOptimizedFile = saveFailedOptimizedFile;

            _fileOptimizerOptions = ConstantsAndConfig.GenerateOptimizerOptions(logLevel);

            if (string.IsNullOrWhiteSpace(_tempDirectory))
            {
                _tempDirectory = FolderHelperMethods.Internal_TempDirectory.Value;
            }
            if (string.IsNullOrWhiteSpace(_failedFilesDirectory))
            {
                _failedFilesDirectory = FolderHelperMethods.Internal_FailedFilesDirectory.Value;
            }

            Directory.CreateDirectory(_tempDirectory);
            Directory.CreateDirectory(_failedFilesDirectory);
        }

        public async Task OptimizeFile(OptimizableFile file)
        {
            var w = Stopwatch.StartNew();

            var tempFiles = new List<string>();

            var errors = new List<string>();

            try
            {
                Console.WriteLine($"=== Optimizing image: {file.Path} ===");

                var fileName = Path.GetFileName(file.Path);
                var tempFilePath = Path.Combine(_tempDirectory, RandomFileNameHelper.RandomizeFileName(fileName));
                tempFiles.Add(tempFilePath);

                await AsyncFileHelper.CopyFileAsync(file.Path, tempFilePath, true);

                Orientation? jpegFileOrientation = null;
                bool shouldUseJpgWorkaround = FileTypeHelper.IsJpgFile(tempFilePath);
                if (shouldUseJpgWorkaround)
                {
                    jpegFileOrientation = await ExifImageRotator.UnrotateImageAsync(tempFilePath);
                }

                var args = _fileOptimizerOptions;

                //This next line should disable showing the window, but apparently it doesn't work yet as of version 13.50.2431
                //if (!_shouldShowFileOptimizerWindow)
                //{
                //    args = $"/NoWindow {args}";
                //}
                var processStartInfo = new ProcessStartInfo(_pathToFileOptimizer, $" {args} \"{tempFilePath}\"");
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
                }
                else if (errors.Count != 0)
                {
                    if (_saveFailedOptimizedFile)
                    {
                        var newFilePath = Path.Combine(_failedFilesDirectory, fileName);

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
                file.SetSuccess(optimizedFileSize, w.Elapsed);
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
