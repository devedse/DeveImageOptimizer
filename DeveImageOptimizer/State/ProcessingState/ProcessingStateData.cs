using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DeveImageOptimizer.ProcessingState
{
    [Serializable]
    public class ProcessingStateData : INotifyPropertyChanged
    {
        public string ProcessingDirectory { get; set; } = string.Empty;
        public List<string> ProcessedFiles { get; set; } = new List<string>();
        public List<string> FailedFiles { get; set; } = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
