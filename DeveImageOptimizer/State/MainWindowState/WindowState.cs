using DeveImageOptimizer.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DeveImageOptimizer.State.MainWindowState
{
    [Serializable]
    public class WindowState : INotifyPropertyChanged
    {
        public string ProcessingDirectory { get; set; } = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
