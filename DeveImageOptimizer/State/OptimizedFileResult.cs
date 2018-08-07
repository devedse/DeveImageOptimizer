using System;
using System.Collections.Generic;

namespace DeveImageOptimizer.State
{
    public class OptimizedFileResult
    {
        public string Path { get; }
        public string RelativePath { get; }

        public OptimizationResult OptimizationResult { get; }

        public long OriginalSize { get; }
        public long OptimizedSize { get; }

        public TimeSpan Duration { get; }

        public IEnumerable<string> Errors { get; }

        public OptimizedFileResult(string path, string relativepath, OptimizationResult optimizationResult, long originalSize, long optimizedSize, TimeSpan duration, List<string> errors)
        {
            Path = path;
            RelativePath = relativepath;
            OptimizationResult = optimizationResult;
            OriginalSize = originalSize;
            OptimizedSize = optimizedSize;
            Duration = duration;
            Errors = errors;
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
