namespace DeveImageOptimizer.State
{
    public interface IFilesProcessedListener
    {
        void AddProcessedFile(OptimizableFile optimizedFileResult);
    }
}
