using System;
using System.Collections.Generic;
using System.IO;
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

        public static IEnumerable<string> RecurseFiles(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
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
    }
}
