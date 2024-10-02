using System;
using System.Windows;
using Microsoft.Win32;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Attaches
{
    public enum ThemeType
    {
        SyncWithSystem,
        Blue2019,
        Dark2019,
        Light2019,
        Blue2022,
        Dark2022,
        Light2022
    }

    public class XamlThemeDictionary : ResourceDictionary
    {
        public XamlThemeDictionary()
        {
            _instance = this;
            UpdateTheme(Theme);
        }

        private const string UIPath = "pack://application:,,,/OhmStudio.UI;component/";

        public static event EventHandler ThemeChanged;

        private static XamlThemeDictionary _instance;
        public static XamlThemeDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("The XamlThemeResource is not loaded!");
                }
                return _instance;
            }
        }

        private ThemeType _theme;
        public ThemeType Theme
        {
            get => _theme;
            set
            {
                if (_theme == value)
                {
                    return;
                }
                UpdateTheme(_theme = value);
                ThemeChanged?.Invoke(this, EventArgs.Empty);
                StatusManager.Update();
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                SyncWithSystemTheme();
            }
        }

        private void SyncWithSystemTheme()
        {
            UpdateTheme(SystemHelper.DetermineIfInLightThemeMode ? ThemeType.Light2022 : ThemeType.Dark2022);
        }

        private void UpdateTheme(ThemeType themeType)
        {
            if (themeType == ThemeType.SyncWithSystem)
            {
                SyncWithSystemTheme();
                SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
                SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
                return;
            }
            if (Theme != ThemeType.SyncWithSystem)
            {
                SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
            }
            string url = themeType switch
            {
                ThemeType.Blue2019 => "VisualStudio2019/BlueTheme.xaml",
                ThemeType.Dark2019 => "VisualStudio2019/DarkTheme.xaml",
                ThemeType.Light2019 => "VisualStudio2019/LightTheme.xaml",
                ThemeType.Blue2022 => "VisualStudio2022/BlueTheme.xaml",
                ThemeType.Dark2022 => "VisualStudio2022/DarkTheme.xaml",
                _ => "VisualStudio2022/LightTheme.xaml"
            };
            url = $"{UIPath}Themes/{url}";

            if (MergedDictionaries.Count == 0)
            {
                MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(url) });
                MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UIPath + "Styles/VisualStudio.xaml") });
            }
            else
            {
                MergedDictionaries[0] = new ResourceDictionary() { Source = new Uri(url) };
            }
        }
    }
}