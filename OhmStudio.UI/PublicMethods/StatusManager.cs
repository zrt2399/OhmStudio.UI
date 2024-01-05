using System;
using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.PublicMethods
{
    public static class StatusManager
    {
        static SolidColorBrush StatusBarBackgroundRunning => (SolidColorBrush)Application.Current.Resources[nameof(StatusBarBackgroundRunning)];
        static SolidColorBrush WindowBorderBrushRunning => (SolidColorBrush)Application.Current.Resources[nameof(WindowBorderBrushRunning)];
        static SolidColorBrush StatusBarBackgroundDefault => (SolidColorBrush)Application.Current.Resources[nameof(StatusBarBackgroundDefault)];
        static SolidColorBrush WindowBorderBrushDefault => (SolidColorBrush)Application.Current.Resources[nameof(WindowBorderBrushDefault)];

        public static event EventHandler IsRunningChanged;

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
                        Application.Current.Resources["StatusBarBackground"] = StatusBarBackgroundRunning;
                        Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = WindowBorderBrushRunning;
                    }
                    else
                    {
                        Application.Current.Resources["StatusBarBackground"] = StatusBarBackgroundDefault;
                        Application.Current.Resources["EnvironmentMainWindowActiveDefaultBorder"] = WindowBorderBrushDefault;
                    }
                }
                IsRunningChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        public static void Update()
        {
            IsRunning = IsRunning;
        }
    }
}