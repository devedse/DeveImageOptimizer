using DeveCoolLib.Collections.Concurrent;
using DeveImageOptimizer.ImageOptimization;

namespace DeveImageOptimizer.State
{
    public class FileProcessedStateRemembererFile
    {
        public ImageOptimizationLevel OptimizationLevel { get; set; }
        public ConcurrentHashSet<string> ConcurrentHashSet { get; }

        public FileProcessedStateRemembererFile(ImageOptimizationLevel optimizationLevel)
        {
            OptimizationLevel = optimizationLevel;
            ConcurrentHashSet = new ConcurrentHashSet<string>();
        }
    }
}
