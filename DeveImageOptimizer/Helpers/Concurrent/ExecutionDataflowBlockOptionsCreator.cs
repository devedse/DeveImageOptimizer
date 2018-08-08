using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DeveImageOptimizer.Helpers.Concurrent
{
    public static class ExecutionDataflowBlockOptionsCreator
    {
        public static ExecutionDataflowBlockOptions SynchronizeForUiThread(ExecutionDataflowBlockOptions executionDataflowBlockOptions)
        {
            if (SynchronizationContext.Current != null)
            {
                executionDataflowBlockOptions.TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            return executionDataflowBlockOptions;
        }
    }
}
