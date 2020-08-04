using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOptimization;
using DeveImageOptimizer.State.SqlState;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public class SqlProcessedStateRememberer : IFileProcessedState
    {
        public bool ShouldAlwaysOptimize { get; set; }

        private readonly string _filePath;

        public SqlProcessedStateRememberer(bool shouldAlwaysOptimize, string saveFilePath = null)
        {
            ShouldAlwaysOptimize = shouldAlwaysOptimize;

            _filePath = saveFilePath;
            if (_filePath == null)
            {
                _filePath = Path.Combine(FolderHelperMethods.ConfigFolder, ConstantsAndConfig.ProcessedFilesSqlDbName);
            }

            var dbContext = CreateDbContext();
            dbContext.Database.EnsureCreated();
        }

        private ProcessedFilesDbContext CreateDbContext()
        {
            return new ProcessedFilesDbContext(_filePath);
        }

        public async Task AddFullyOptimizedFile(string path, ImageOptimizationLevel imageOptimizationLevel)
        {
            var hash = Sha512HashCalculator.CalculateFileHash(path);
            var fileName = Path.GetFileName(path);

            using (var dbContext = CreateDbContext())
            {
                var foundEntry = dbContext.ProcessedFiles.FirstOrDefault(t => t.Hash == hash);

                if (foundEntry == null)
                {
                    var newEntry = new ProcessedFile()
                    {
                        Hash = hash,
                        ImageOptimizationLevel = (int)imageOptimizationLevel,
                        FileNames = fileName
                    };
                    dbContext.ProcessedFiles.Add(newEntry);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    if (foundEntry.FileNames == null)
                    {
                        foundEntry.FileNames = "";
                    }

                    var fileNames = foundEntry.FileNames.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();

                    bool shouldSave = false;

                    if (!fileNames.Contains(fileName))
                    {
                        fileNames.Add(fileName);
                        foundEntry.FileNames = string.Join("|", fileNames);
                        shouldSave = true;
                    }

                    if (foundEntry.ImageOptimizationLevel < (int)imageOptimizationLevel)
                    {
                        foundEntry.ImageOptimizationLevel = (int)imageOptimizationLevel;
                        shouldSave = true;
                    }

                    if (shouldSave)
                    {
                        await dbContext.SaveChangesAsync();
                    }
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
            using (var dbContext = CreateDbContext())
            {
                var foundEntry = dbContext.ProcessedFiles.FirstOrDefault(t => t.Hash == hash && t.ImageOptimizationLevel >= (int)imageOptimizationLevel);
                if (foundEntry == null)
                {
                    return true;
                }
            }
            Console.WriteLine($"File {path} is already optimized. File hash: {hash}");
            return false;
        }
    }
}
