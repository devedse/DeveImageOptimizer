using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using DeveImageOptimizer.ImageOptimization;
using SixLabors.ImageSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public class FileProcessedStateRememberer : IFileProcessedState
    {
        public bool ShouldAlwaysOptimize { get; set; }

        private readonly ConcurrentDictionary<string, FileProcessedStateRemembererFile> _fullyOptimizedFileHashes = new ConcurrentDictionary<string, FileProcessedStateRemembererFile>();
        private readonly string _filePath;

        public FileProcessedStateRememberer(bool shouldAlwaysOptimize, string? saveFilePath = null)
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;

            if (saveFilePath == null)
            {
                _filePath = Path.Combine(FolderHelperMethods.ConfigFolder, ConstantsAndConfig.ProcessedFilesFileName);
            }
            else
            {
                _filePath = saveFilePath;
            }

            using (var streamReader = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var splitted = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length == 2 || splitted.Length == 3)
                            {
                                string hash = splitted[0];
                                if (hash.Length == Sha512HashCalculator.ExpectedHashLength)
                                {
                                    //Previous default before this was configurable
                                    ImageOptimizationLevel optimizationLevel = ImageOptimizationLevel.Maximum;
                                    if (splitted.Length == 3)
                                    {
                                        Enum.TryParse<ImageOptimizationLevel>(splitted[2], out optimizationLevel);
                                    }

                                    var listOfFiles = _fullyOptimizedFileHashes.GetOrAdd(hash, (hash) => new FileProcessedStateRemembererFile(optimizationLevel));

                                    if ((int)optimizationLevel > (int)listOfFiles.OptimizationLevel)
                                    {
                                        listOfFiles.OptimizationLevel = optimizationLevel;
                                    }

                                    var files = splitted[1];
                                    var splittedFiles = files.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var file in splittedFiles)
                                    {
                                        if (file.Length < 2000)
                                        {
                                            //Ignore files that have paths that are incredibly long, this is more likely a corrupt line
                                            listOfFiles.ConcurrentHashSet.Add(file);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing line:{Environment.NewLine}{line}{Environment.NewLine}Exception: {ex}");
                    }
                }
            }
        }

        public Task AddFullyOptimizedFile(string path, ImageOptimizationLevel imageOptimizationLevel)
        {
            var hash = Sha512HashCalculator.CalculateFileHash(path);
            var fileName = Path.GetFileName(path);

            var setForThisHash = _fullyOptimizedFileHashes.GetOrAdd(hash, (hash) => new FileProcessedStateRemembererFile(imageOptimizationLevel));

            var itemNewlyCreated = setForThisHash.ConcurrentHashSet.TryAdd(fileName);

            if (itemNewlyCreated)
            {
                AppendToFile(hash, fileName, imageOptimizationLevel);
            }
            else
            {
                if ((int)setForThisHash.OptimizationLevel < (int)imageOptimizationLevel)
                {
                    setForThisHash.OptimizationLevel = imageOptimizationLevel;
                    AppendToFile(hash, fileName, imageOptimizationLevel);
                }
            }

            return Task.CompletedTask;
        }

        private readonly object _saveFileLockject = new object();

        private void AppendToFile(string hash, string fileName, ImageOptimizationLevel imageOptimizationLevel)
        {
            lock (_saveFileLockject)
            {
                using (var writer = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine($"{hash}|{fileName}|{imageOptimizationLevel}");
                }
            }
        }

        public bool ShouldOptimizeFile(string path, ImageOptimizationLevel imageOptimizationLevel)
        {
            if (ShouldAlwaysOptimize)
            {
                return true;
            }

            var hash = Sha512HashCalculator.CalculateFileHash(path);

            if (_fullyOptimizedFileHashes.TryGetValue(hash, out var foundValue))
            {
                if ((int)foundValue.OptimizationLevel < (int)imageOptimizationLevel)
                {
                    Console.WriteLine($"File {path} was previously optimized using a weaker optimization level: {foundValue.OptimizationLevel}. Re-optimizing...");
                    return true;
                }
                else
                {
                    Console.WriteLine($"File {path} is already optimized. File hash: {hash}");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void SaveAndCleanupProcessedFilesDataAndInitNewWriter()
        {
            lock (_saveFileLockject)
            {
                using (var writer = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    foreach (var curhash in _fullyOptimizedFileHashes)
                    {
                        foreach (var file in curhash.Value.ConcurrentHashSet)
                        {
                            var stringToWrite = $"{curhash.Key}|{file}|{curhash.Value.OptimizationLevel}";
                            writer.WriteLine(stringToWrite);
                        }
                    }
                }
            }
        }
    }
}
