﻿using DeveImageOptimizer.Helpers;
using ExifLibrary;
using System.Diagnostics;
using System.IO;
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

        public async Task<bool> OptimizeFile(string fileToOptimize, bool saveFailedOptimizedFile = false)
        {
            var fileName = Path.GetFileName(fileToOptimize);
            var tempFilePath = Path.Combine(_tempDirectory, RandomFileNameHelper.RandomizeFileName(fileName));

            await AsyncFileHelper.CopyFileAsync(fileToOptimize, tempFilePath, true);

            Orientation jpegFileOrientation = Orientation.Normal;
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

            var imagesEqual = await ImageComparer2.AreImagesEqualAsync(fileToOptimize, tempFilePath);

            if (imagesEqual)
            {
                await AsyncFileHelper.CopyFileAsync(tempFilePath, fileToOptimize, true);
            }
            else
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

            File.Delete(tempFilePath);
            return imagesEqual;
        }
    }
}
