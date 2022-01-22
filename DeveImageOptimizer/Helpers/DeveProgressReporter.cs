using System;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    public class DeveProgressReporter<T>
    {
        private readonly Func<T, Task>? _reportProgress;
        private readonly bool _useSynchronizationContext;
        private readonly IProgress<T>? _progress;

        public DeveProgressReporter(Func<T, Task>? reportProgress, bool useSynchronizationContext)
        {
            _useSynchronizationContext = useSynchronizationContext;

            if (reportProgress != null)
            {
                _reportProgress = reportProgress;
                var progress = new Progress<T>(async (t) => await reportProgress(t));
                _progress = progress;
            }
        }

        public async Task Report(T value)
        {
            if (_reportProgress != null && _progress != null)
            {
                if (_useSynchronizationContext)
                {
                    _progress.Report(value);
                }
                else
                {
                    await _reportProgress(value);
                }
            }
        }
    }
}
