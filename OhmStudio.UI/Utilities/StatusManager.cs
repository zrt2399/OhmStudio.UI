using System;
using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.Utilities
{
    public class StatusManager
    {
        private SolidColorBrush StatusBarBackgroundRunning => (SolidColorBrush)Application.Current.Resources[nameof(StatusBarBackgroundRunning)];
        private SolidColorBrush WindowBorderBrushRunning => (SolidColorBrush)Application.Current.Resources[nameof(WindowBorderBrushRunning)];
        private SolidColorBrush StatusBarBackgroundDefault => (SolidColorBrush)Application.Current.Resources[nameof(StatusBarBackgroundDefault)];
        private SolidColorBrush WindowBorderBrushDefault => (SolidColorBrush)Application.Current.Resources[nameof(WindowBorderBrushDefault)];

        public static event EventHandler IsRunningChanged;

        public static StatusManager Current { get; } = new StatusManager();

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value)
                {
                    return;
                }
                _isRunning = value;
                Update(value);
                IsRunningChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Update()
        {
            Update(IsRunning);
        }

        private void Update(bool isRunning)
        {
            if (Application.Current != null)
            {
                if (isRunning)
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
        }
    }
}