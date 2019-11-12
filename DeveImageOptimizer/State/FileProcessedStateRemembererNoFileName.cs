using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public class FileProcessedStateRemembererNoFileName : IFileProcessedState
    {
        public bool ShouldAlwaysOptimize { get; }

        private readonly HashSet<string> _fullyOptimizedFileHashes = new HashSet<string>();
        private readonly string _filePath;

        public FileProcessedStateRemembererNoFileName(bool shouldAlwaysOptimize, string saveFilePath = null)
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
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        _fullyOptimizedFileHashes.Add(line);
                    }
                }
            }
        }

        public async Task AddFullyOptimizedFile(string path)
        {
            var hash = Sha512HashCalculator.CalculateFileHash(path);
            var addedHash = _fullyOptimizedFileHashes.Add(hash);
            if (addedHash)
            {
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
                        streamWriter.WriteLine(curhash);
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

            var hash = Sha512HashCalculator.CalculateFileHash(path);
            if (!_fullyOptimizedFileHashes.Contains(hash))
            {
                Console.WriteLine($"File hash of file that is already optimized: {hash}");
                return true;
            }
            return false;
        }

        Task IFileProcessedState.AddFullyOptimizedFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
