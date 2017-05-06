using DeveImageOptimizer.State.MainWindowState;
using DeveImageOptimizer.State.UserSettings;

namespace DeveImageOptimizer.State
{
    public static class StaticState
    {
        public static StateManager<UserSettingsData> UserSettingsManager { get; } = new StateManager<UserSettingsData>("UserSettings.xml");
        public static StateManager<WindowState> WindowStateManager { get; } = new StateManager<WindowState>("WindowState.xml");
        public static StateManager<FilesProcessingState> FilesProcessingStateManager { get; } = new StateManager<FilesProcessingState>("FilesProcessingState.xml");
    }
}
