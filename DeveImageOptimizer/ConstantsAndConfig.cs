using System;

namespace DeveImageOptimizer
{
    public static class ConstantsAndConfig
    {
        public static string[] ValidExtensions { get; } = new string[] { ".PNG", ".JPG", ".JPEG", ".GIF", ".BMP" };
        public const string AppDataDirectoryName = "DeveImageOptimizer";
        public const string TempDirectoryName = "DeveImageOptimizerTemp";
        public const string FailedFilesDirectoryName = "FailedFiles";

        public const string ProcessedFilesSqlDbName = "ProcessedFiles.db";
        public const string ProcessedFilesFileName = "ProcessedFiles.txt";
        public const string ProcessedDirsFileName = "ProcessedDirs.txt";

        public static string GenerateOptimizerOptions(int logLevel)
        {
            if (logLevel < 0 || logLevel > 4)
            {
                throw new ArgumentException("LogLevel should be between 0 and 4", nameof(logLevel));
            }
            return OptimizerOptions.Replace("{LogLevel}", logLevel.ToString());
        }

        private const string OptimizerOptions =
            "/BMPCopyMetadata=true " +
            "/CSSEnableTidy=false " +
            "/CSSTemplate=low " +
            "/EXEDisablePETrim=false " +
            "/EXEEnableUPX=false " +
            "/GIFCopyMetadata=true " +
            "/GIFAllowLossy=false " +
            "/GZCopyMetadata=true " +
            "/HTMLEnableTidy=false " +
            "/JPEGCopyMetadata=true " +
            "/JPEGUseArithmeticEncoding=false " +
            "/JPEGAllowLossy=false " +
            "/JSEnableJSMin=false " +
            "/JSAdditionalExtensions= " +
            "/LUAEnableLeanify=false " +
            "/MiscCopyMetadata=true " +
            "/MP3CopyMetadata=true " +
            "/MP4CopyMetadata=true " +
            "/PCXCopyMetadata=true " +
            "/PDFProfile=none " +
            "/PDFCustomDPI=150 " +
            "/PDFSkipLayered=false " +
            "/PNGCopyMetadata=true " +
            "/PNGAllowLossy=false " +
            "/TIFFCopyMetadata=true " +
            "/XMLEnableLeanify=false " +
            "/ZIPCopyMetadata=true " +
            "/ZIPRecurse=false " +
            "/KeepAttributes=false " +
            "/DoNotUseRecycleBin=true " +
            "/IncludeMask= " +
            "/ExcludeMask= " +
            "/DisablePluginMask= " +
            "/BeepWhenDone=false " +
            "/ShutdownWhenDone=false " +
            "/AlwaysOnTop=false " +
            "/AllowDuplicates=false " +
            "/AllowMultipleInstances=true " +
            "/EnableCache=false " +
            "/Level=9 " +
            "/ProcessPriority=16384 " +
            "/CheckForUpdates=1 " +
            "/LogLevel={LogLevel} " +
            "/FilenameFormat=0 " +
            "/LeanifyIterations=-1 " +
            "/Theme=Windows " +
            //"/TempDirectory= " +
            //"/Version=10.00.1878 " +
            "/TGACopyMetadata=true " +
            //"/Donator= " +
            //"/Donation= " +
            "/ShowToolBar=true " +
            "/ClearWhenComplete=false " +
            "/HideAds=true " +
            "/Language=0 " +
            "/DoNotCreateBackups=true " +
            "/WAVCopyMetadata=false " +
            "/WAVStripSilence=false " +
            "/PNGWolfIterations=-1 " +
            "/StartupDelay=1000";
    }
}
