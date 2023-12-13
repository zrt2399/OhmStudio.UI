using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.PublicMethods
{
    public static class StatusManager
    {
        static Color RunningStatusBarBackground => (Color)Application.Current.Resources["RunningStatusBarBackground"];
        static Color RunningWindowBorderBrush => (Color)Application.Current.Resources["RunningWindowBorderBrush"];
        static Color DefaultStatusBarBackground => (Color)Application.Current.Resources["DefaultStatusBarBackground"];
        static Color DefaultWindowBorderBrush => (Color)Application.Current.Resources["DefaultWindowBorderBrush"];

        private static bool _isRunning;
        public static bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                if (value)
                {
                    Application.Current.Resources["StatusBarBackground"] = new SolidColorBrush(RunningStatusBarBackground);
                    Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = new SolidColorBrush(RunningWindowBorderBrush);
                }
                else
                {
                    Application.Current.Resources["StatusBarBackground"] = new SolidColorBrush(DefaultStatusBarBackground);
                    Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = new SolidColorBrush(DefaultWindowBorderBrush);
                }
            }
        }

        public static void Update()
        {
            IsRunning = IsRunning;
        }
    }
}