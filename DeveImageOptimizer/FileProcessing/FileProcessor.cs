using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task<IEnumerable<OptimizedFileResult>> ProcessDirectoryParallel(string directory)
        {
            var optimizedFileResultsForThisDirectory = new List<OptimizedFileResult>();



            var lockject = new object();

            var extractIenumerableBlock = new TransformManyBlock<IEnumerable<string>, string>(t => t);

            var bufferBlock = new BufferBlock<string>(new DataflowBlockOptions()
            {
                BoundedCapacity = 10
            });

            //System.Threading.Tasks.Dataflow.

            var processFileBlock = new TransformBlock<string, OptimizedFileResult>(async file =>
            {
                var optimizedFileResult = new OptimizedFileResult(file, file, OptimizationResult.Success, 1, 1, TimeSpan.FromSeconds(1), new List<string>());
                //var optimizedFileResult = await ProcessFile(file, directory);
                if (_fileProcessedListener != null)
                {
                    //lock (lockject)
                    //{
                    await Task.Delay(2000);
                    _fileProcessedListener.AddProcessedFile(optimizedFileResult);
                    //}
                }
                return optimizedFileResult;
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 4,
                BoundedCapacity = 10

            });

            var putInListBlock = new ActionBlock<OptimizedFileResult>(t =>
            {
                Console.WriteLine($"Ik ga een file in dit block stoppen: {t}");
                optimizedFileResultsForThisDirectory.Add(t);
                //Thread.Sleep(5000);
                Console.WriteLine($"Done: {t}");
            }, new ExecutionDataflowBlockOptions()
            {
                TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
            });

            extractIenumerableBlock.LinkTo(bufferBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            bufferBlock.LinkTo(processFileBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            processFileBlock.LinkTo(putInListBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            var files = RecurseFiles(directory).Where(t => Constants.ValidExtensions.Contains(Path.GetExtension(t).ToUpperInvariant()));



            if (false)
            {
                var result = await extractIenumerableBlock.SendAsync(files.Take(100));
                Console.WriteLine($"Posting complete: {result}");
            }
            else
            {
                await Task.Run(async () =>
                {
                    foreach (var item in files.Take(100))
                    {
                        Console.WriteLine($"Posting: {Path.GetFileName(item)}");
                        var result = await bufferBlock.SendAsync(item);

                        if (!result)
                        {
                            Console.WriteLine("Result is false!!!");
                        }
                        Thread.Sleep(100);
                    }
                });
            }
            
            Console.WriteLine("Completing");
            extractIenumerableBlock.Complete();
            await putInListBlock.Completion;

            //IEnumerable<OptimizedFileResult> concattedRetVal = optimizedFileResultsForThisDirectory;

            //var directories = Directory.GetDirectories(directory);
            //foreach (var subDirectory in directories)
            //{
            //    var results = await ProcessDirectoryParallel(subDirectory);
            //    concattedRetVal = concattedRetVal.Concat(results);
            //}

            return optimizedFileResultsForThisDirectory;
        }

        public IEnumerable<string> RecurseFiles(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                Console.WriteLine($"Returning file from RecurseFiles: {file}");
                yield return file;
            }

            var directories = Directory.GetDirectories(directory);
            foreach (var subDirectory in directories)
            {
                var recursedFIles = RecurseFiles(subDirectory);
                foreach (var subFile in recursedFIles)
                {
                    yield return subFile;
                }
            }
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
