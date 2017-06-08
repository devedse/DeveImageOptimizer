using System;
using System.Collections.Generic;
using System.Text;

namespace DeveImageOptimizer.State
{
    public interface IFilesProcessingState
    {
        void AddProcessedFile(string file);
        void AddFailedFile(string file);
    }
}
