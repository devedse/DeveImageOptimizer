using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DeveImageOptimizer.ProcessingState
{
    [Serializable]
    public class ProcessingStateData : INotifyPropertyChanged
    {
        public string ProcessingDirectory { get; set; } = string.Empty;
        public ObservableCollection<string> ProcessedFiles { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FailedFiles { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
