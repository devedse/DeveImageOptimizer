using DeveImageOptimizer.ProcessingState;
using DeveImageOptimizer.State.UserSettings;

namespace DeveImageOptimizer.State
{
    public static class StaticState
    {
        public static StateManager<UserSettingsData> UserSettingsManager { get; } = new StateManager<UserSettingsData>("UserSettings.xml");
        public static StateManager<ProcessingStateData> ProcessingStateManager { get; } = new StateManager<ProcessingStateData>("ProcessingState.xml");
    }
}
