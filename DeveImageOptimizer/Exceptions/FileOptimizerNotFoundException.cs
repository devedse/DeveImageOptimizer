using System;

namespace DeveImageOptimizer.Exceptions
{
    public class FileOptimizerNotFoundException : Exception
    {
        public FileOptimizerNotFoundException(string fileOptimizerPath) :
            base($"Could not find FileOptimizer.exe in path: '{fileOptimizerPath}'{Environment.NewLine}{Environment.NewLine}Please ensure FileOptimizer is installed before optimizing images. It can be downloaded here:{Environment.NewLine}https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/{Environment.NewLine}Or installed through Chocolatey:{Environment.NewLine}https://chocolatey.org/packages/FileOptimizer")
        {
        }
    }
}
