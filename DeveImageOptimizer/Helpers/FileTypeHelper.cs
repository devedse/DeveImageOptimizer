using System;
using System.IO;
using System.Linq;

namespace DeveImageOptimizer.Helpers
{
    public static class FileTypeHelper
    {
        public static bool IsJpgFile(string file)
        {
            var extension = Path.GetExtension(file);
            if (ConstantsFileExtensions.JPGExtensions.Contains(extension.ToUpperInvariant()))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidImageFile(string file)
        {
            var result = ConstantsFileExtensions.AllValidExtensions.Contains(Path.GetExtension(file).ToUpperInvariant());
            return result;
        }
    }
}
