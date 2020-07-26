using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using DeveImageOptimizer.State;
using DeveImageOptimizer.State.StoringProcessedDirectories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DeveImageOptimizer.FileProcessing
{
    public class DeveImageOptimizerProcessor
    {
        private readonly FileOptimizerProcessor _fileOptimizer;
        private readonly DeveImageOptimizerConfiguration _configuration;

        //private readonly IFilesProcessedListener _fileProcessedListener;
        private readonly IFileProcessedState _fileProcessedState;
        private readonly IDirProcessedState _dirProcessedState;

        private readonly DeveProgressReporter<OptimizableFile> _progress;
        private readonly DeveProgressReporter<int> _progressCountTotalFiles;

        public DeveImageOptimizerProcessor(DeveImageOptimizerConfiguration configuration, IProgressReporter progressReporter, IFileProcessedState fileProcessedState, IDirProcessedState dirProcessedState)
        {
            _fileOptimizer = new FileOptimizerProcessor(configuration);
            _configuration = configuration;
            //_fileProcessedListener = fileProcessedListener;
            _fileProcessedState = fileProcessedState;
            _dirProcessedState = dirProcessedState;

            var progress = new Progress<OptimizableFile>();
            var progressCountTotalFiles = new Progress<int>();



            if (progressReporter != null)
            {
                _progress = new DeveProgressReporter<OptimizableFile>(progressReporter.OptimizableFileProgressUpdated, !configuration.AwaitProgressReporting);
                _progressCountTotalFiles = new DeveProgressReporter<int>(progressReporter.TotalFileCountDiscovered, !configuration.AwaitProgressReporting);
            }
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

        public Task<IEnumerable<OptimizableFile>> ProcessDirectory(string directory)
        {
            if (!_configuration.ExecuteImageOptimizationParallel)
            {
                return ProcessDirectorySingleThreaded(directory);
            }
            else
            {
                return ProcessDirectoryParallel(directory);
            }
        }

        private async Task<IEnumerable<OptimizableFile>> ProcessDirectorySingleThreaded(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizableFile>();
            var filesProcessedPerDirectoryCounter = new Dictionary<string, int>();

            var files = FileHelperMethods.RecurseFiles(directory, FileTypeHelper.IsValidImageFile);

            if (_configuration.DetermineCountFilesBeforehand)
            {
                var tempFiles = files.ToList();
                await _progressCountTotalFiles.Report(tempFiles.Count);
                files = tempFiles;
            }

            int fileCount = 0;
            foreach (var file in files)
            {
                fileCount++;
                var optimizedFileResult = await ProcessFile(file.FilePath, directory);

                optimizedFileResultsForThisDirectory.Add(optimizedFileResult);

                await DetectIfDirectoryIsCompleteAndThenAddToDirState(filesProcessedPerDirectoryCounter, file, optimizedFileResult);
            }

            if (!_configuration.DetermineCountFilesBeforehand)
            {
                await _progressCountTotalFiles.Report(fileCount);
            }

            Console.WriteLine($"Optimization of directory {directory} completed.");

            return optimizedFileResultsForThisDirectory;
        }

        private async Task<IEnumerable<OptimizableFile>> ProcessDirectoryParallel(string directory)
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
                MaxDegreeOfParallelism = _configuration.MaxDegreeOfParallelism,
                BoundedCapacity = _configuration.MaxDegreeOfParallelism * 2,
                EnsureOrdered = false
            });

            var putInListBlock = new ActionBlock<OptimizedFileResultAndOriginalFile>(async t =>
            {
                optimizedFileResultsForThisDirectory.Add(t.OptimizedFileResult);

                await DetectIfDirectoryIsCompleteAndThenAddToDirState(filesProcessedPerDirectoryCounter, t.FileAndCountOfFilesInDirectory, t.OptimizedFileResult);
            }, ExecutionDataflowBlockOptionsCreator.SynchronizeForUiThread(new ExecutionDataflowBlockOptions()
            {
                EnsureOrdered = false,
                MaxDegreeOfParallelism = 1
            }));

            processFileBlock.LinkTo(putInListBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            var files = FileHelperMethods.RecurseFiles(directory, FileTypeHelper.IsValidImageFile);

            if (_configuration.DetermineCountFilesBeforehand)
            {
                var tempFiles = files.ToList();
                await _progressCountTotalFiles.Report(tempFiles.Count);
                files = tempFiles;
            }

            int fileCount = 0;
            await Task.Run(async () =>
            {
                foreach (var file in files)
                {
                    fileCount++;
                    Console.WriteLine($"Posting: {Path.GetFileName(file.FilePath)}");
                    var result = await processFileBlock.SendAsync(file);

                    if (!result)
                    {
                        Console.WriteLine("Result is false!!!");
                    }
                }
            });

            if (!_configuration.DetermineCountFilesBeforehand)
            {
                await _progressCountTotalFiles.Report(fileCount);
            }

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

            await _progress.Report(optimizableFile);

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

            await _progress.Report(optimizableFile);

            return optimizableFile;
        }
    }
}
