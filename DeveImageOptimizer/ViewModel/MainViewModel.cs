using DeveImageOptimizer.FileProcessing;
using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ProcessingState;
using DeveImageOptimizer.State;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

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

            GoCommand = new RelayCommand(async () => await GoCommandImp(), () => true);
        }

        private void ProcessingStateData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StaticState.ProcessingStateManager.Save();
        }

        public ICommand GoCommand { get; private set; }
        private async Task GoCommandImp()
        {
            var fileOptimizer = new FileOptimizerProcessor(StaticState.UserSettingsManager.State.FileOptimizerPath, Path.Combine(FolderHelperMethods.AssemblyDirectory.Value, Constants.TempDirectoryName));
            var fileProcessor = new FileProcessor(fileOptimizer, ProcessingStateData);
            await fileProcessor.ProcessDirectory(ProcessingStateData.ProcessingDirectory);

        }
    }
}