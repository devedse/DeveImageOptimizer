namespace DeveImageOptimizer.State
{
    public interface IProgressReporter
    {
        void OptimizableFileProgressUpdated(OptimizableFile optimizableFile);
    }
}
