using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public class FileProcessedStateRememberer : IFileProcessedState
    {
        public bool ShouldAlwaysOptimize { get; set; }

        private readonly ConcurrentDictionary<string, ConcurrentHashSet<string>> _fullyOptimizedFileHashes = new ConcurrentDictionary<string, ConcurrentHashSet<string>>();
        private readonly string _filePath;

        public FileProcessedStateRememberer(bool shouldAlwaysOptimize, string saveFilePath = null)
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;

            _filePath = saveFilePath;
            if (_filePath == null)
            {
                _filePath = Path.Combine(FolderHelperMethods.ConfigFolder, ConstantsAndConfig.ProcessedFilesFileName);
            }

            using (var streamReader = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var splitted = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length == 2)
                            {
                                string hash = splitted[0];
                                if (hash.Length == Sha512HashCalculator.ExpectedHashLength)
                                {
                                    var listOfFiles = _fullyOptimizedFileHashes.GetOrAdd(hash, (hash) => new ConcurrentHashSet<string>());

                                    var files = splitted[1];
                                    var splittedFiles = files.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var file in splittedFiles)
                                    {
                                        if (file.Length < 2000)
                                        {
                                            //Ignore files that have paths that are incredibly long, this is more likely a corrupt line
                                            listOfFiles.Add(file);
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

        public Task AddFullyOptimizedFile(string path)
        {
            var hash = Sha512HashCalculator.CalculateFileHash(path);
            var fileName = Path.GetFileName(path);

            var setForThisHash = _fullyOptimizedFileHashes.GetOrAdd(hash, (hash) => new ConcurrentHashSet<string>());

            var itemNewlyCreated = setForThisHash.TryAdd(fileName);

            if (itemNewlyCreated)
            {
                AppendToFile(hash, fileName);
            }

            return Task.CompletedTask;
        }

        private readonly object _saveFileLockject = new object();

        private void AppendToFile(string hash, string fileName)
        {
            lock (_saveFileLockject)
            {
                using (var writer = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine($"{hash}|{fileName}");
                }
            }
        }

        public bool ShouldOptimizeFile(string path)
        {
            if (ShouldAlwaysOptimize)
            {
                return true;
            }

            var hash = Sha512HashCalculator.CalculateFileHash(path);
            if (!_fullyOptimizedFileHashes.ContainsKey(hash))
            {
                return true;
            }
            Console.WriteLine($"File {path} is already optimized. File hash: {hash}");
            return false;
        }

        public void SaveAndCleanupProcessedFilesDataAndInitNewWriter()
        {
            lock (_saveFileLockject)
            {
                using (var writer = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    foreach (var curhash in _fullyOptimizedFileHashes)
                    {
                        foreach (var file in curhash.Value)
                        {
                            var stringToWrite = $"{curhash.Key}|{file}";
                            writer.WriteLine(stringToWrite);
                        }
                    }
                }
            }
        }
    }
}
