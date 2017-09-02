using System;
using System.Collections.Generic;

namespace DeveImageOptimizer.State
{
    public class OptimizedFileResult
    {
        public string Path { get; }
        public bool Successful { get; }
        public bool SkippedBecausePreviouslyOptimized { get; }

        public long OriginalSize { get; }
        public long OptimizedSize { get; }

        public TimeSpan Duration { get; }

        public IEnumerable<string> Errors { get; }

        public OptimizedFileResult(string path, bool successful, bool skippedBecausePreviouslyOptimized, long originalSize, long optimizedSize, TimeSpan duration, List<string> errors)
        {
            Path = path;
            Successful = successful;
            SkippedBecausePreviouslyOptimized = skippedBecausePreviouslyOptimized;
            OriginalSize = originalSize;
            OptimizedSize = optimizedSize;
            Duration = duration;
            Errors = errors;
        }
    }
}
