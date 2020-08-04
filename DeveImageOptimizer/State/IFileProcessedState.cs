using DeveImageOptimizer.ImageOptimization;
using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public interface IFileProcessedState
    {
        bool ShouldOptimizeFile(string path, ImageOptimizationLevel imageOptimizationLevel);
        Task AddFullyOptimizedFile(string path, ImageOptimizationLevel imageOptimizationLevel);
    }
}