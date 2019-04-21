using System.Threading.Tasks;

namespace DeveImageOptimizer.State.StoringProcessedDirectories
{
    public interface IDirProcessedState
    {
        bool ShouldOptimizeFileInDirectory(string path);
        Task AddFullyOptimizedDirectory(string path);
    }
}
