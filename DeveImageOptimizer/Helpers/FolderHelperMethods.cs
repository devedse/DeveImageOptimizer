using System;
using System.IO;
using System.Reflection;

namespace DeveImageOptimizer.Helpers
{
    public static class FolderHelperMethods
    {
        public static Lazy<string> EntryAssemblyDirectory { get; } = new Lazy<string>(() => CreateEntryAssemblyDirectory());

        private static string CreateEntryAssemblyDirectory()
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            return assemblyDir;
        }

        public static Lazy<string> EntryTempDirectory { get; } = new Lazy<string>(() => Path.Combine(EntryAssemblyDirectory.Value, ConstantsAndConfig.TempDirectoryName));

        public static Lazy<string> LocationOfImageProcessorDllAssemblyDirectory { get; } = new Lazy<string>(() => CreateLocationOfImageProcessorAssemblyDirectory());

        private static string CreateLocationOfImageProcessorAssemblyDirectory()
        {
            var assembly = typeof(FolderHelperMethods).GetTypeInfo().Assembly;
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            return assemblyDir;
        }

        public static Lazy<string> LocationOfImageProcessorDllAssemblyTempDirectory { get; } = new Lazy<string>(() => Path.Combine(LocationOfImageProcessorDllAssemblyDirectory.Value, ConstantsAndConfig.TempDirectoryName));
    }
}
