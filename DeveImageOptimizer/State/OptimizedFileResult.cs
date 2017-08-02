using System.Collections.Generic;

namespace DeveImageOptimizer.State
{
    public class OptimizedFileResult
    {
        public string Path { get; }
        public bool Successful { get; }

        public long OriginalSize { get; }
        public long OptimizedSize { get; }

        public List<string> Errors { get; set; }

        public OptimizedFileResult(string path, bool successful, long originalSize, long optimizedSize, List<string> errors)
        {
            Path = path;
            Successful = successful;
            OriginalSize = originalSize;
            OptimizedSize = optimizedSize;
            Errors = errors;
        }
    }
}
