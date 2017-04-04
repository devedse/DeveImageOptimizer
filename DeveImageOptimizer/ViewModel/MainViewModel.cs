using DeveImageOptimizer.ProcessingState;
using DeveImageOptimizer.State;
using GalaSoft.MvvmLight;

namespace DeveImageOptimizer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ProcessingStateData ProcessingStateData { get; set; }

        public MainViewModel()
        {
            ProcessingStateData = StaticState.ProcessingStateManager.State;

            ProcessingStateData.PropertyChanged += ProcessingStateData_PropertyChanged;
        }

        private void ProcessingStateData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StaticState.ProcessingStateManager.Save();
        }
    }
}