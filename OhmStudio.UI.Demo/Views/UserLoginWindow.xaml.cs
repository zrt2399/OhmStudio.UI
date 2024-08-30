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
        public UserLoginWindow()
        {
            InitializeComponent();
        }

        Random _random = new Random();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeType themeType = XamlThemeDictionary.Instance.Theme;
            while (themeType == XamlThemeDictionary.Instance.Theme)
            {
                themeType = (ThemeType)_random.Next(0, Enum.GetValues(typeof(ThemeType)).Length);
            }
            XamlThemeDictionary.Instance.Theme = themeType;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UIMessageTip.Show("这是一个提示\r\n这是二个提示");
            DialogResult = true;
        }
    }
}