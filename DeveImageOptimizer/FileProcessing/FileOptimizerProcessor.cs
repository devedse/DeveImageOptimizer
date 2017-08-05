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
        private string _pathToFileOptimizer;
        private string _tempDirectory;

        public FileOptimizerProcessor(string pathToFileOptimizer, string tempDirectory)
        {
            _pathToFileOptimizer = pathToFileOptimizer;
            _tempDirectory = tempDirectory;

            Directory.CreateDirectory(tempDirectory);
        }

        public async Task<OptimizedFileResult> OptimizeFile(string fileToOptimize, bool saveFailedOptimizedFile = false)
        {
            long originalFileSize = new FileInfo(fileToOptimize).Length;

            var tempFiles = new List<string>();
            bool imagesEqual = false;

            var errors = new List<string>();
            var w = Stopwatch.StartNew();

            try
            {
                Console.WriteLine();
                Console.WriteLine($"Optimizing image: {fileToOptimize}");

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

                var processStartInfo = new ProcessStartInfo(_pathToFileOptimizer, tempFilePath)
                {
                    CreateNoWindow = true,
                };
                //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //processStartInfo.CreateNoWindow = true;

                await ProcessRunner.RunProcessAsync(processStartInfo);

                if (shouldUseJpgWorkaround)
                {
                    await ExifImageRotator.RerotateImageAsync(tempFilePath, jpegFileOrientation);
                }

                imagesEqual = await ImageComparer2.AreImagesEqualAsync(fileToOptimize, tempFilePath);

                if (imagesEqual)
                {
                    await AsyncFileHelper.CopyFileAsync(tempFilePath, fileToOptimize, true);
                }
                else
                {
                    errors.Add("Optimized image isn't equal to source image.");

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

            //The fileToOptimize has been overwritten by the optimized file, so this is the optimized file size.
            long optimizedFileSize = new FileInfo(fileToOptimize).Length;

            var optimizedFileResult = new OptimizedFileResult(fileToOptimize, imagesEqual, originalFileSize, optimizedFileSize, errors);

            if (errors.Any())
            {
                Console.WriteLine("Errors:");
                foreach(var error in errors)
                {
                    Console.WriteLine($"\tError: {error}");
                }
            }
            Console.WriteLine($"Image optimization completed. Result: {imagesEqual}");

            return optimizedFileResult;
        }
    }
}
