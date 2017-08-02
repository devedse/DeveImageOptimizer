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
    }
}
