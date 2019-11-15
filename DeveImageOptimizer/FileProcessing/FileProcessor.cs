using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DeveImageOptimizer.FileProcessing
{
    public class FileProcessor
    {
        private readonly FileOptimizerProcessor _fileOptimizer;
        private readonly IFilesProcessedListener _fileProcessedListener;
        private readonly IFileProcessedState _fileProcessedState;
        private readonly IDirProcessedState _dirProcessedState;

        public FileProcessor(FileOptimizerProcessor fileOptimizer, IFilesProcessedListener fileProcessedListener, IFileProcessedState fileProcessedState, IDirProcessedState dirProcessedState)
        {
            _fileOptimizer = fileOptimizer;
            _fileProcessedListener = fileProcessedListener;
            _fileProcessedState = fileProcessedState;
            _dirProcessedState = dirProcessedState;
        }

        public static int IncreaseCountInDict(Dictionary<string, int> dict, string key)
        {
            dict.TryGetValue(key, out int val);
            val += 1;
            dict[key] = val;
            return val;
        }

        private async Task DetectIfDirectoryIsCompleteAndThenAddToDirState(Dictionary<string, int> filesProcessedPerDirectoryCounter, FileAndCountOfFilesInDirectory file, OptimizableFile optimizedFileResult)
        {
            if (optimizedFileResult.OptimizationResult == OptimizationResult.Success || optimizedFileResult.OptimizationResult == OptimizationResult.Skipped)
            {
                var result = IncreaseCountInDict(filesProcessedPerDirectoryCounter, file.DirectoryPath);
                if (result >= file.CountOfFilesInDirectory)
                {
                    await _dirProcessedState.AddFullyOptimizedDirectory(file.DirectoryPath);
                }
            }
        }

        public async Task<IEnumerable<OptimizableFile>> ProcessDirectory(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizableFile>();
            var filesProcessedPerDirectoryCounter = new Dictionary<string, int>();

            var files = FileHelperMethods.RecurseFiles(directory, FileTypeHelper.IsValidImageFile);

            foreach (var file in files)
            {
                var optimizedFileResult = await ProcessFile(file.FilePath, directory);
                if (_fileProcessedListener != null)
                {
                    _fileProcessedListener.AddProcessedFile(optimizedFileResult);
                }

                optimizedFileResultsForThisDirectory.Add(optimizedFileResult);

                await DetectIfDirectoryIsCompleteAndThenAddToDirState(filesProcessedPerDirectoryCounter, file, optimizedFileResult);
            }

            Console.WriteLine($"Optimization of directory {directory} completed.");

            return optimizedFileResultsForThisDirectory;
        }

        public async Task<IEnumerable<OptimizableFile>> ProcessDirectoryParallel(string directory, int maxDegreeOfParallelism = 4)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizableFile>();
            var filesProcessedPerDirectoryCounter = new Dictionary<string, int>();

            var processFileBlock = new TransformBlock<FileAndCountOfFilesInDirectory, OptimizedFileResultAndOriginalFile>(async file =>
            {
                var optimizedFileResult = await ProcessFile(file.FilePath, directory);
                return new OptimizedFileResultAndOriginalFile()
                {
                    FileAndCountOfFilesInDirectory = file,
                    OptimizedFileResult = optimizedFileResult
                };
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                BoundedCapacity = 10,
                EnsureOrdered = false
            });

            var putInListBlock = new ActionBlock<OptimizedFileResultAndOriginalFile>(async t =>
            {
                if (_fileProcessedListener != null)
                {
                    _fileProcessedListener.AddProcessedFile(t.OptimizedFileResult);
                }
                optimizedFileResultsForThisDirectory.Add(t.OptimizedFileResult);

                await DetectIfDirectoryIsCompleteAndThenAddToDirState(filesProcessedPerDirectoryCounter, t.FileAndCountOfFilesInDirectory, t.OptimizedFileResult);
            }, ExecutionDataflowBlockOptionsCreator.SynchronizeForUiThread(new ExecutionDataflowBlockOptions()
            {
                EnsureOrdered = false,
                MaxDegreeOfParallelism = 1
            }));

            processFileBlock.LinkTo(putInListBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            var files = FileHelperMethods.RecurseFiles(directory, FileTypeHelper.IsValidImageFile);
            await Task.Run(async () =>
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"Posting: {Path.GetFileName(file.FilePath)}");
                    var result = await processFileBlock.SendAsync(file);

                    if (!result)
                    {
                        Console.WriteLine("Result is false!!!");
                    }
                }
            });

            Console.WriteLine("Completing");
            processFileBlock.Complete();
            await putInListBlock.Completion;

            Console.WriteLine($"Optimization of directory {directory} completed.");

            return optimizedFileResultsForThisDirectory;
        }

        private async Task<OptimizableFile> ProcessFile(string file, string originDirectory)
        {
            Console.WriteLine();
            var fileSize = new FileInfo(file).Length;
            var optimizableFile = new OptimizableFile(file, RelativePathFinderHelper.GetRelativePath(originDirectory, file), fileSize);

            if (_dirProcessedState.ShouldOptimizeFileInDirectory(file) && _fileProcessedState.ShouldOptimizeFile(file))
            {
                await _fileOptimizer.OptimizeFile(optimizableFile);

                //If the file is successfully optimized add it to the list of optimized files so it can be skipped next time
                if (optimizableFile.OptimizationResult == OptimizationResult.Success)
                {
                    await _fileProcessedState.AddFullyOptimizedFile(optimizableFile.Path);
                }
            }
            else
            {
                Console.WriteLine($"=== Skipping because already optimized: {file} ===");

                optimizableFile.SetSkipped();
            }
            return optimizableFile;
        }
    }
}
