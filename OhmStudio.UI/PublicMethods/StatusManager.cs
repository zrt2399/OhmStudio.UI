using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.PublicMethods
{
    public static class StatusManager
    {
        static SolidColorBrush RunningStatusBarBackground => (SolidColorBrush)Application.Current.Resources["RunningStatusBarBackground"];
        static SolidColorBrush RunningWindowBorderBrush => (SolidColorBrush)Application.Current.Resources["RunningWindowBorderBrush"];
        static SolidColorBrush DefaultStatusBarBackground => (SolidColorBrush)Application.Current.Resources["DefaultStatusBarBackground"];
        static SolidColorBrush DefaultWindowBorderBrush => (SolidColorBrush)Application.Current.Resources["DefaultWindowBorderBrush"];

        private static bool _isRunning;
        public static bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                if (Application.Current != null)
                {
                    if (value)
                    {
                        Application.Current.Resources["StatusBarBackground"] = RunningStatusBarBackground;
                        Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = RunningWindowBorderBrush;
                    }
                    else
                    {
                        Application.Current.Resources["StatusBarBackground"] = DefaultStatusBarBackground;
                        Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = DefaultWindowBorderBrush;
                    }
                }
            }
        }

        public static void Update()
        {
            IsRunning = IsRunning;
        }
    }
}