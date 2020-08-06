using System.Diagnostics;
using System.Threading.Tasks;

namespace DeveImageOptimizer.Helpers
{
    internal static class ProcessRunnerOld
    {
        public static Task<int> RunProcessAsync(ProcessStartInfo processStartInfo)
        {
            //Console.WriteLine($"> {Path.GetFileName(processStartInfo.FileName)} {processStartInfo.Arguments}");

            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }
    }
}
