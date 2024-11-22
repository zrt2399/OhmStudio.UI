using System;
using System.Windows;
using Microsoft.Win32;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Utilities;

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
        private const string UIPath = "pack://application:,,,/OhmStudio.UI;component/";

        public static event EventHandler ThemeChanged;

        public XamlThemeDictionary()
        {
            _current = this;
            UpdateTheme(Theme);
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        private static XamlThemeDictionary _current;
        public static XamlThemeDictionary Current
        {
            get
            {
                if (_current == null)
                {
                    throw new InvalidOperationException("The XamlThemeResource is not loaded!");
                }
                return _current;
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
                StatusManager.Current.Update();
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
            if (Theme == ThemeType.SyncWithSystem)
            {
                UpdateTheme(SystemHelper.DetermineIfInLightThemeMode ? ThemeType.Light2022 : ThemeType.Dark2022);
            }
        }

        private void UpdateTheme(ThemeType themeType)
        {
            if (themeType == ThemeType.SyncWithSystem)
            {
                SyncWithSystemTheme();  
                return;
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