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
        public bool ShouldAlwaysOptimize { get; }

        private readonly ConcurrentDictionary<string, ConcurrentHashSet<string>> _fullyOptimizedFileHashes = new ConcurrentDictionary<string, ConcurrentHashSet<string>>();
        private readonly string _filePath;

        public const string FileNameHashesStorage = "ProcessedFiles.txt";

        public FileProcessedStateRememberer(bool shouldAlwaysOptimize)
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;
            _filePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, FileNameHashesStorage);

            using (var streamReader = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate)))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var listOfFiles = new ConcurrentHashSet<string>();

                            var splitted = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            string hash = splitted[0];

                            if (splitted.Length > 1)
                            {
                                var files = splitted[1];

                                var splittedFiles = files.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                listOfFiles = new ConcurrentHashSet<string>(splittedFiles);
                            }
                            _fullyOptimizedFileHashes.TryAdd(hash, listOfFiles);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing line:{Environment.NewLine}{line}{Environment.NewLine}Exception: {ex}");
                    }
                }
            }
        }

        public async Task AddFullyOptimizedFile(string path)
        {
            var hash = FileHashCalculator.CalculateFileHash(path);
            var fileName = Path.GetFileName(path);

            var itemNewlyCreated = _fullyOptimizedFileHashes.TryAdd(hash, new ConcurrentHashSet<string>() { fileName });
            if (!itemNewlyCreated)
            {
                if (_fullyOptimizedFileHashes.TryGetValue(hash, out ConcurrentHashSet<string> fileList))
                {
                    bool shouldSave = fileList.Add(fileName);
                    if (shouldSave)
                    {
                        await SaveToFile();
                    }
                }
            }
            else
            {
                await SaveToFile();
            }
        }

        private readonly object _saveFileLockject = new object();

        private Task SaveToFile()
        {
            return Task.Run(() =>
            {
                lock (_saveFileLockject)
                {
                    using (var streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                    {
                        foreach (var curhash in _fullyOptimizedFileHashes)
                        {
                            var combinedFileNames = string.Join(":", curhash.Value);
                            var stringToWrite = $"{curhash.Key}|{combinedFileNames}";
                            streamWriter.WriteLine(stringToWrite);
                        }
                    }
                }
            });
        }

        public bool ShouldOptimizeFile(string path)
        {
            if (ShouldAlwaysOptimize)
            {
                return true;
            }

            var hash = FileHashCalculator.CalculateFileHash(path);
            if (!_fullyOptimizedFileHashes.ContainsKey(hash))
            {
                return true;
            }
            Console.WriteLine($"File {path} is already optimized. File hash: {hash}");
            return false;
        }
    }
}
