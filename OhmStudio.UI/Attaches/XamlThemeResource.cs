using System;
using System.Windows;
using Microsoft.Win32;
using OhmStudio.UI.Helpers;

namespace OhmStudio.UI.Attaches
{
    public enum ThemeType
    {
        VS2019Blue,
        VS2019Dark,
        VS2019Light,
        VS2022Blue,
        VS2022Dark,
        VS2022Light
    }

    public class XamlThemeDictionary : ResourceDictionary
    {
        public XamlThemeDictionary()
        {
            _instance = this;
            UpdateTheme(Theme, true);
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

        private ThemeType _theme = ThemeType.VS2022Blue;
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
                ThemeChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        private bool _isSyncWithSystem;
        public bool IsSyncWithSystem
        {
            get => _isSyncWithSystem;
            set
            {
                if (_isSyncWithSystem == value)
                {
                    return;
                }
                _isSyncWithSystem = value;
                if (value)
                {
                    SyncWithSystemTheme();
                    SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
                }
                else
                {
                    SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
                }
            }
        }

        void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                SyncWithSystemTheme();
            }
        }

        void SyncWithSystemTheme()
        {
            Theme = SystemHelper.DetermineIfInLightThemeMode ? ThemeType.VS2022Light : ThemeType.VS2022Dark;
        }

        void UpdateTheme(ThemeType themeType, bool isInitialisation = false)
        {
            string url = themeType switch
            {
                ThemeType.VS2019Blue => "VisualStudio2019/BlueTheme.xaml",
                ThemeType.VS2019Dark => "VisualStudio2019/DarkTheme.xaml",
                ThemeType.VS2019Light => "VisualStudio2019/LightTheme.xaml",
                ThemeType.VS2022Blue => "VisualStudio2022/BlueTheme.xaml",
                ThemeType.VS2022Dark => "VisualStudio2022/DarkTheme.xaml",
                _ => "VisualStudio2022/LightTheme.xaml"
            };
            url = $"{UIPath}Themes/{url}";

            if (isInitialisation)
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