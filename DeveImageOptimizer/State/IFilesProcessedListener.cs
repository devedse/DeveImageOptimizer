using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public interface IProgressReporter
    {
        Task TotalFileCountDiscovered(int count);
        Task OptimizableFileProgressUpdated(OptimizableFile optimizableFile);
    }
}
