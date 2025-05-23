﻿using DeveCoolLib.Collections.Concurrent;
using DeveImageOptimizer.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State.StoringProcessedDirectories
{
    public class DirProcessedStateRememberer : IDirProcessedState
    {
        public bool ShouldAlwaysOptimize { get; set; }

        private readonly ConcurrentHashSet<string> _fullyOptimizedDirectories = new ConcurrentHashSet<string>();
        private readonly string _filePath;

        public DirProcessedStateRememberer(bool shouldAlwaysOptimize, string? saveFilePath = null)
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;

            if (saveFilePath == null)
            {
                _filePath = Path.Combine(FolderHelperMethods.ConfigFolder, ConstantsAndConfig.ProcessedDirsFileName);
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

        public Task AddFullyOptimizedDirectory(string path)
        {
            var lowerPath = path.ToLowerInvariant();
            var itemNewlyCreated = _fullyOptimizedDirectories.TryAdd(lowerPath);
            if (itemNewlyCreated)
            {
                AppendToFile(lowerPath);
            }

            return Task.CompletedTask;
        }

        private readonly object _saveFileLockject = new object();

        private void AppendToFile(string path)
        {
            lock (_saveFileLockject)
            {
                using (var writer = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine($"{path}");
                }
            }
        }

        public bool ShouldOptimizeFileInDirectory(string path)
        {
            if (ShouldAlwaysOptimize)
            {
                return true;
            }

            var dirPath = Path.GetDirectoryName(path)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(dirPath) || !_fullyOptimizedDirectories.Contains(dirPath))
            {
                return true;
            }
            Console.WriteLine($"Directory {dirPath} is already optimized.");
            return false;
        }
    }
}
