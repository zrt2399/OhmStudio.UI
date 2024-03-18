using System;
using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public enum OhmTheme
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
            instance = this;
            Update(Theme, true);
        }

        private const string UIPath = "/OhmStudio.UI;component/";

        public static event EventHandler ThemeChanged;

        private static XamlThemeDictionary instance;
        public static XamlThemeDictionary Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("The XamlThemeResource is not loaded!");
                }
                return instance;
            }
        }

        private OhmTheme theme = OhmTheme.VS2022Blue;
        public OhmTheme Theme
        {
            get => theme;
            set
            {
                if (theme == value)
                {
                    return;
                }
                Update(theme = value, false);
                ThemeChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        void Update(OhmTheme theme, bool isInitialisation)
        {
            string url = theme switch
            {
                OhmTheme.VS2019Blue => "VisualStudio2019/BlueTheme.xaml",
                OhmTheme.VS2019Dark => "VisualStudio2019/DarkTheme.xaml",
                OhmTheme.VS2019Light => "VisualStudio2019/LightTheme.xaml",
                OhmTheme.VS2022Blue => "VisualStudio2022/BlueTheme.xaml",
                OhmTheme.VS2022Dark => "VisualStudio2022/DarkTheme.xaml",
                _ => "VisualStudio2022/LightTheme.xaml"
            };
            url = $"{UIPath}Themes/{url}";

            if (isInitialisation)
            {
                MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(url, UriKind.Relative) });
                MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UIPath + "Styles/VisualStudio.xaml", UriKind.Relative) });
            }
            else
            {
                MergedDictionaries[0] = new ResourceDictionary() { Source = new Uri(url, UriKind.Relative) };
            }
        }
    }
}