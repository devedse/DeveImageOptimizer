﻿using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.Helpers.Concurrent;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State.StoringProcessedDirectories
{
    public class DirProcessedStateRememberer : IDirProcessedState
    {
        public bool ShouldAlwaysOptimize { get; }
        public string FileNameHashesStorage { get; }

        private readonly ConcurrentHashSet<string> _fullyOptimizedDirectories = new ConcurrentHashSet<string>();
        private readonly string _filePath;

        public DirProcessedStateRememberer(bool shouldAlwaysOptimize, string fileNameHashesStorage = "ProcessedDirs.txt")
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;
            FileNameHashesStorage = fileNameHashesStorage;

            _filePath = Path.Combine(FolderHelperMethods.LocationOfImageProcessorDllAssemblyDirectory.Value, FileNameHashesStorage);

            using (var streamReader = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            _fullyOptimizedDirectories.TryAdd(line);

                            var listOfFiles = new ConcurrentHashSet<string>();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while parsing line:{Environment.NewLine}{line}{Environment.NewLine}Exception: {ex}");
                    }
                }
            }
        }

        public async Task AddFullyOptimizedDirectory(string path)
        {
            var itemNewlyCreated = _fullyOptimizedDirectories.TryAdd(path.ToLowerInvariant());
            if (itemNewlyCreated)
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
                        foreach (var curDirPath in _fullyOptimizedDirectories)
                        {
                            streamWriter.WriteLine(curDirPath);
                        }
                    }
                }
            });
        }

        public bool ShouldOptimizeFileInDirectory(string path)
        {
            if (ShouldAlwaysOptimize)
            {
                return true;
            }

            var dirPath = Path.GetDirectoryName(path).ToLowerInvariant();

            if (!_fullyOptimizedDirectories.Contains(dirPath))
            {
                return true;
            }
            Console.WriteLine($"Directory {dirPath} is already optimized.");
            return false;
        }
    }
}