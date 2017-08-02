namespace DeveImageOptimizer.State
{
    public interface IFilesProcessingState
    {
        void AddProcessedFile(OptimizedFileResult optimizedFileResult);
    }
}
