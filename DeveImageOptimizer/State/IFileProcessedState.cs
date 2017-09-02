using System.Threading.Tasks;

namespace DeveImageOptimizer.State
{
    public interface IFileProcessedState
    {
        bool ShouldOptimizeFile(string path);
        Task AddFullyOptimizedFile(string path);
    }
}