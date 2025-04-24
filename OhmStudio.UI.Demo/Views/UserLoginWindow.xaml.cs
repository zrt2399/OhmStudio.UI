using System;
using System.Windows;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Messaging;

namespace OhmStudio.UI.Demo.Views
{
    /// <summary>
    /// UserLoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginWindow : ChromeWindow
    {
        private Random _random = new Random();
        public UserLoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeType themeType = XamlThemeDictionary.Current.Theme;
            while (themeType == XamlThemeDictionary.Current.Theme)
            {
                themeType = (ThemeType)_random.Next(0, Enum.GetValues(typeof(ThemeType)).Length);
            }
            XamlThemeDictionary.Current.Theme = themeType;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageTip.Show("这是一个提示\r\n这是二个提示");
            DialogResult = true;
        }
    }
}