using System.IO;

namespace DeveImageOptimizer.Helpers
{
    public static class RandomFileNameHelper
    {
        public static string RandomizeFileName(string fileName, string desiredExtension = null)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (desiredExtension == null)
            {
                desiredExtension = Path.GetExtension(fileName);
            }
            var newFileName = $"{ nameWithoutExtension}_{Path.ChangeExtension(Path.GetRandomFileName(), desiredExtension)}";
            return newFileName;
        }
    }
}
