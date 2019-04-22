using DeveImageOptimizer.State.StoringProcessedDirectories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeveImageOptimizer.Helpers
{
    public static class FileHelperMethods
    {
        public static void SafeDeleteTempFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Couldn't remove tempfile at path: '{path}'. Exception:{Environment.NewLine}{ex}");
            }
        }

        public static IEnumerable<FileAndCountOfFilesInDirectory> RecurseFiles(string directory, Func<string, bool> filter = null)
        {
            if (filter == null)
            {
                filter = t => true;
            }

            var files = Directory.GetFiles(directory).Where(filter).ToList();

            foreach (var file in files)
            {
                yield return new FileAndCountOfFilesInDirectory()
                {
                    FilePath = file,
                    DirectoryPath = directory,
                    CountOfFilesInDirectory = files.Count
                };
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
    }
}
