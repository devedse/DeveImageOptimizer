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
            if (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidImageFile(string file)
        {
            var result = ConstantsAndConfig.ValidExtensions.Contains(Path.GetExtension(file).ToUpperInvariant());
            return result;
        }
    }
}
