﻿using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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

        public async Task<IEnumerable<OptimizedFileResult>> ProcessDirectory(string directory, bool processParallel)
        {
            if (!processParallel)
            {
                return await ProcessDirectorySingleThreaded(directory);
            }
            else
            {
                return await ProcessDirectoryParallel(directory);
            }
        }

        private async Task<IEnumerable<OptimizedFileResult>> ProcessDirectorySingleThreaded(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizedFileResult>();

            var files = FileHelperMethods.RecurseFiles(directory);

            foreach (var file in files)
            {
                if (Constants.ValidExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                {
                    var optimizedFileResult = await ProcessFile(file, directory);
                    if (_fileProcessedListener != null)
                    {
                        _fileProcessedListener.AddProcessedFile(optimizedFileResult);
                    }

                    optimizedFileResultsForThisDirectory.Add(optimizedFileResult);
                }
            }

            return optimizedFileResultsForThisDirectory;
        }

        private async Task<IEnumerable<OptimizedFileResult>> ProcessDirectoryParallel(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizedFileResult>();

            var processFileBlock = new TransformBlock<string, OptimizedFileResult>(async file =>
            {
                var optimizedFileResult = await ProcessFile(file, directory);
                return optimizedFileResult;
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 4,
                BoundedCapacity = 10
            });

            var putInListBlock = new ActionBlock<OptimizedFileResult>(t =>
            {
                if (_fileProcessedListener != null)
                {
                    _fileProcessedListener.AddProcessedFile(t);
                }
                optimizedFileResultsForThisDirectory.Add(t);
            }, ExecutionDataflowBlockOptionsCreator.SynchronizeForUiThread(new ExecutionDataflowBlockOptions()
            {

            }));

            processFileBlock.LinkTo(putInListBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            var files = FileHelperMethods.RecurseFiles(directory);
            await Task.Run(async () =>
            {
                foreach (var file in files)
                {
                    if (Constants.ValidExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                    {
                        Console.WriteLine($"Posting: {Path.GetFileName(file)}");
                        var result = await processFileBlock.SendAsync(file);

                        if (!result)
                        {
                            Console.WriteLine("Result is false!!!");
                        }
                    }
                }
            });

            Console.WriteLine("Completing");
            processFileBlock.Complete();
            await putInListBlock.Completion;

            return optimizedFileResultsForThisDirectory;
        }

        private async Task<OptimizedFileResult> ProcessFile(string file, string originDirectory)
        {
            Console.WriteLine();
            if (_fileProcessedState.ShouldOptimizeFile(file))
            {
                var optimizedFileResult = await _fileOptimizer.OptimizeFile(file, originDirectory);

                //If the file is successfully optimized add it to the list of optimized files so it can be skipped next time
                if (optimizedFileResult.OptimizationResult == OptimizationResult.Success)
                {
                    await _fileProcessedState.AddFullyOptimizedFile(optimizedFileResult.Path);
                }

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
