using System;
using System.IO;
using System.Reflection;

namespace DeveImageOptimizer.Helpers
{
    public static class FolderHelperMethods
    {
        //public static Lazy<string> EntryAssemblyDirectory { get; } = new Lazy<string>(() => CreateEntryAssemblyDirectory());

        //private static string CreateEntryAssemblyDirectory()
        //{
        //    var assembly = Assembly.GetEntryAssembly();
        //    var assemblyDir = Path.GetDirectoryName(assembly.Location);
        //    return assemblyDir;
        //}

        //public static Lazy<string> EntryTempDirectory { get; } = new Lazy<string>(() => Path.Combine(EntryAssemblyDirectory.Value, ConstantsAndConfig.TempDirectoryName));

        /// <summary>
        /// This is the folder all configuration / state stuff should be stored in
        /// </summary>
        public static string ConfigFolder => Internal_AppDataFolder.Value;
        /// <summary>
        /// This is the folder contains all the Temp files
        /// </summary>
        public static string TempDirectory => Internal_TempDirectory.Value;
        /// <summary>
        /// This folder contains all the Failed Files
        /// </summary>
        public static string FailedFilesDirectory => Internal_FailedFilesDirectory.Value;

        public static Lazy<string> Internal_AppDataFolder { get; } = new Lazy<string>(() => EnsureExists(CreateAppDataFolder()));
        public static Lazy<string> Internal_AssemblyDirectory { get; } = new Lazy<string>(() => EnsureExists(CreateLocationOfImageProcessorAssemblyDirectory()));
        public static Lazy<string> Internal_TempDirectory { get; } = new Lazy<string>(() => EnsureExists(Path.Combine(Path.GetTempPath(), ConstantsAndConfig.TempDirectoryName)));
        public static Lazy<string> Internal_FailedFilesDirectory { get; } = new Lazy<string>(() => EnsureExists(Path.Combine(ConfigFolder, ConstantsAndConfig.FailedFilesDirectoryName)));

        public static Lazy<string> Internal_TempForTestDirectory { get; } = new Lazy<string>(() => EnsureExists(Path.Combine(Internal_AssemblyDirectory.Value, "TempForTest")));

        private static string CreateAppDataFolder()
        {
            var appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var combinedPath = Path.Combine(appdataPath, ConstantsAndConfig.AppDataDirectoryName);
            return combinedPath;
        }

        private static string CreateLocationOfImageProcessorAssemblyDirectory()
        {
            var assembly = typeof(FolderHelperMethods).GetTypeInfo().Assembly;
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            return assemblyDir;
        }

        private static string EnsureExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}
