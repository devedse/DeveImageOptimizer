namespace DeveImageOptimizer
{
    public static class Constants
    {
        public static string[] ValidExtensions { get; } = new string[] { ".PNG", ".JPG", ".JPEG", ".GIF", ".BMP" };
        public const string TempDirectoryName = "Temp";
        
        public const string VipsDir = @".nuget\packages\vipsnuget\1.0.5\tools";
    }
}
