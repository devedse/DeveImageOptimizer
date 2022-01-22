using System;

namespace DeveImageOptimizer.Exceptions
{
    public class FileOptimizerNotFoundException : Exception
    {
        public FileOptimizerNotFoundException(string? message) : base(message)
        {
        }
    }
}
