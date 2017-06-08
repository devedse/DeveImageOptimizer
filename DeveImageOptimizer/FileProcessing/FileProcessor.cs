using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.FileProcessing
{
    public class FileProcessor
    {
        private FileOptimizerProcessor _fileOptimizer;
        private IFilesProcessingState _processingStateData;

        public FileProcessor(FileOptimizerProcessor fileOptimizer, IFilesProcessingState processingStateData)
        {
            _fileOptimizer = fileOptimizer;
            _processingStateData = processingStateData;
        }

        public async Task ProcessDirectory(string directory)
        {
            await ProcessDirectoryInternal(directory);
        }

        private async Task ProcessDirectoryInternal(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                await ProcessFile(file);
            }

            var directories = Directory.GetDirectories(directory);

            foreach (var subDirectory in directories)
            {
                await ProcessDirectoryInternal(subDirectory);
            }
        }

        private async Task ProcessFile(string file)
        {
            var extension = Path.GetExtension(file).ToUpperInvariant();
            if (Constants.ValidExtensions.Contains(extension))
            {
                var worked = await _fileOptimizer.OptimizeFile(file);

                if (worked)
                {
                    _processingStateData.AddProcessedFile(file);
                }
                else
                {
                    _processingStateData.AddFailedFile(file);
                }
            }
        }
    }
}
