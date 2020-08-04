using DeveImageOptimizer.ImageOptimization;
using System;
using System.Collections.Generic;

namespace DeveImageOptimizer.State
{
    public class OptimizableFile
    {
        public string Path { get; }
        public string RelativePath { get; }

        public OptimizationResult OptimizationResult { get; private set; } = OptimizationResult.InProgress;

        public long OriginalSize { get; }
        public long OptimizedSize { get; private set; }

        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;

        public List<string> Errors { get; } = new List<string>();

        public ImageOptimizationLevel? ImageOptimizationLevel { get; set; }

        public OptimizableFile(string path, string relativepath, long originalSize)
        {
            Path = path;
            RelativePath = relativepath;
            OriginalSize = originalSize;
            OptimizedSize = originalSize;
        }

        public void SetSuccess(long optimizedFileSize, TimeSpan duration, ImageOptimizationLevel imageOptimizationLevel)
        {
            OptimizationResult = OptimizationResult.Success;
            OptimizedSize = optimizedFileSize;
            Duration = duration;
            ImageOptimizationLevel = imageOptimizationLevel;
        }

        public void SetFailed(TimeSpan duration, IEnumerable<string> errors)
        {
            OptimizationResult = OptimizationResult.Failed;
            Duration = duration;
            Errors.AddRange(errors);
        }

        public void SetSkipped()
        {
            OptimizationResult = OptimizationResult.Skipped;
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
