namespace DeveImageOptimizer.State
{
    public interface IProgressReporter
    {
        void TotalFileCountDiscovered(int count);
        void OptimizableFileProgressUpdated(OptimizableFile optimizableFile);
    }
}
