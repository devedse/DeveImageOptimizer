using System.IO;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public static class AsyncFileHelper
    {
        public static async Task CopyFileAsync(string sourceFile, string destinationFile, bool overwrite = false)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                using (var destinationStream = new FileStream(destinationFile, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }
    }
}
