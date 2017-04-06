using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public async Task<bool> OptimizeFile(string fileToOptimize)
        {
            var fileName = Path.GetFileName(fileToOptimize);
            var tempFilePath = Path.Combine(_tempDirectory, fileName);

            await AsyncFileHelper.CopyFileAsync(fileToOptimize, tempFilePath, true);

            var processStartInfo = new ProcessStartInfo(_pathToFileOptimizer, tempFilePath);
            await ProcessRunner.RunProcessAsync(processStartInfo);

            return await ImageComparer.AreImagesEqualAsync(fileToOptimize, tempFilePath);
        }
    }
}
