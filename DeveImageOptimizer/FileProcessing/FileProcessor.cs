using DeveImageOptimizer.Helpers;
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
        private readonly FileOptimizerProcessor _fileOptimizer;
        private readonly IFilesProcessedListener _fileProcessedListener;
        private readonly IFileProcessedState _fileProcessedState;

        public FileProcessor(FileOptimizerProcessor fileOptimizer, IFilesProcessedListener fileProcessedListener, IFileProcessedState fileProcessedState)
        {
            _fileOptimizer = fileOptimizer;
            _fileProcessedListener = fileProcessedListener;
            _fileProcessedState = fileProcessedState;
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
                    var optimizedFileResult = await ProcessFile(file, directory);
                    if (_fileProcessedListener != null)
                    {
                        _fileProcessedListener.AddProcessedFile(optimizedFileResult);
                    }

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

        private async Task<OptimizedFileResult> ProcessFile(string file, string originDirectory)
        {
            Console.WriteLine();
            if (_fileProcessedState.ShouldOptimizeFile(file))
            {
                var optimizedFileResult = await _fileOptimizer.OptimizeFile(file, originDirectory);

                await _fileProcessedState.AddFullyOptimizedFile(optimizedFileResult.Path);

                return optimizedFileResult;
            }
            else
            {                
                Console.WriteLine($"=== Skipping because already optimized: {file} ===");

                var fileSize = new FileInfo(file).Length;
                var skippedFile = new OptimizedFileResult(file, RelativePathFinderHelper.GetRelativePath(originDirectory, file), OptimizationResult.Skipped, fileSize, fileSize, TimeSpan.Zero, new List<string>());
                return skippedFile;
            }
        }
    }
}
