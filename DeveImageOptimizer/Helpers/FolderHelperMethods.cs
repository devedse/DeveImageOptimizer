using System;
using System.IO;
using System.Reflection;

namespace DeveImageOptimizer.Helpers
{
    public static class FolderHelperMethods
    {
        public static Lazy<string> AssemblyDirectory { get; private set; } = new Lazy<string>(() => CreateAssemblyDirectory());

        private static string CreateAssemblyDirectory()
        {
            string codeBase = Assembly.GetEntryAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static Lazy<string> TempDirectory { get; private set; } = new Lazy<string>(() => Path.Combine(AssemblyDirectory.Value, Constants.TempDirectoryName));

        public static Lazy<string> AssemblyDirectoryForTests { get; private set; } = new Lazy<string>(() => CreateAssemblyDirectoryForTests());

        private static string CreateAssemblyDirectoryForTests()
        {
            var startupAssembly = typeof(FolderHelperMethods).GetTypeInfo().Assembly;
            var cb = startupAssembly.CodeBase;

            UriBuilder uri = new UriBuilder(cb);
            string path = Uri.UnescapeDataString(uri.Path);
            var assemblyDir = Path.GetDirectoryName(path);

            return assemblyDir;
        }

        public static Lazy<string> TempDirectoryForTests { get; private set; } = new Lazy<string>(() => Path.Combine(AssemblyDirectoryForTests.Value, Constants.TempDirectoryName));
    }
}
