using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public class FileProcessedStateRememberer : IFileProcessedState
    {
        public bool ShouldAlwaysOptimize { get; }

        private readonly Dictionary<string, HashSet<string>> _fullyOptimizedFileHashes = new Dictionary<string, HashSet<string>>();
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
                            var listOfFiles = new HashSet<string>();

                            var splitted = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            string hash = splitted[0];

                            if (splitted.Length > 1)
                            {
                                var files = splitted[1];

                                var splittedFiles = files.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                listOfFiles = new HashSet<string>(splittedFiles);
                            }
                            _fullyOptimizedFileHashes.Add(hash, listOfFiles);
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
            HashSet<string> fileList;

            if (_fullyOptimizedFileHashes.TryGetValue(hash, out fileList))
            {
                bool shouldSave = fileList.Add(fileName);
                if (shouldSave)
                {
                    await SaveToFile();
                }
            }
            else
            {
                _fullyOptimizedFileHashes.Add(hash, new HashSet<string>() { fileName });
                await SaveToFile();
            }
        }

        private Task SaveToFile()
        {
            return Task.Run(() =>
            {
                using (var streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                {
                    foreach (var curhash in _fullyOptimizedFileHashes)
                    {
                        var combinedFileNames = String.Join(":", curhash.Value.AsEnumerable());
                        var stringToWrite = $"{curhash.Key}|{combinedFileNames}";
                        streamWriter.WriteLine(stringToWrite);
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
