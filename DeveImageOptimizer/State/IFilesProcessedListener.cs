namespace DeveImageOptimizer.State
{
    public interface IFilesProcessedListener
    {
        void AddProcessedFile(OptimizedFileResult optimizedFileResult);
    }
}
