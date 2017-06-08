using System;
using System.IO;
using System.Reflection;

namespace DeveImageOptimizer.Helpers
{
    public static class FolderHelperMethods
    {
        public static Lazy<string> AssemblyDirectory = new Lazy<string>(() => CreateAssemblyDirectory());

        private static string CreateAssemblyDirectory()
        {
            string codeBase = Assembly.GetEntryAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
