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

        public async Task<IEnumerable<OptimizedFileResult>> ProcessDirectory(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizedFileResult>();

            var files = Directory.GetFiles(directory);
            
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).ToUpperInvariant();
                if (Constants.ValidExtensions.Contains(extension))
                {
                    var optimizedFileResult = await ProcessFile(file);
                    optimizedFileResultsForThisDirectory.Add(optimizedFileResult);
                }
            }

            IEnumerable<OptimizedFileResult> concattedRetVal = optimizedFileResultsForThisDirectory;

            var directories = Directory.GetDirectories(directory);
            foreach (var subDirectory in directories)
            {
                var results = await ProcessDirectory(subDirectory);
                concattedRetVal = concattedRetVal.Concat(results);
            }

            return concattedRetVal;
        }

        private async Task<OptimizedFileResult> ProcessFile(string file)
        {
            var optimizedFileResult = await _fileOptimizer.OptimizeFile(file);
            if (_processingStateData != null)
            {
                _processingStateData.AddProcessedFile(optimizedFileResult);
            }

            return optimizedFileResult;
        }
    }
}
