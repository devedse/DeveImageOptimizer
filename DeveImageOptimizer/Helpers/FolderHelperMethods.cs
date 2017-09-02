using System;
using System.IO;
using System.Reflection;

namespace DeveImageOptimizer.Helpers
{
    public static class FolderHelperMethods
    {
        public static Lazy<string> EntryAssemblyDirectory { get; private set; } = new Lazy<string>(() => CreateEntryAssemblyDirectory());

        private static string CreateEntryAssemblyDirectory()
        {
            string codeBase = Assembly.GetEntryAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static Lazy<string> EntryTempDirectory { get; private set; } = new Lazy<string>(() => Path.Combine(EntryAssemblyDirectory.Value, Constants.TempDirectoryName));

        public static Lazy<string> LocationOfImageProcessorDllAssemblyDirectory { get; private set; } = new Lazy<string>(() => CreateLocationOfImageProcessorAssemblyDirectory());

        private static string CreateLocationOfImageProcessorAssemblyDirectory()
        {
            var startupAssembly = typeof(FolderHelperMethods).GetTypeInfo().Assembly;
            var cb = startupAssembly.CodeBase;

            UriBuilder uri = new UriBuilder(cb);
            string path = Uri.UnescapeDataString(uri.Path);
            var assemblyDir = Path.GetDirectoryName(path);

            return assemblyDir;
        }

        public static Lazy<string> LocationOfImageProcessorAssemblyTempDirectory { get; private set; } = new Lazy<string>(() => Path.Combine(LocationOfImageProcessorDllAssemblyDirectory.Value, Constants.TempDirectoryName));
    }
}
