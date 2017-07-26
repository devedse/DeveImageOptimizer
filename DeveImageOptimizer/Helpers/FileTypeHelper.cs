using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
