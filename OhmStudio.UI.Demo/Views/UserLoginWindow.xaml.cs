using System;
using System.Windows;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Controls;
using OhmStudio.UI.PublicMethods;

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
        private void Button_Click(object sender,  RoutedEventArgs e)
        {
            XamlThemeDictionary.Instance.Theme = (OhmTheme)_random.Next(0, 6);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UIMessageTip.ShowWarning("这是一个提示");
            DialogResult = true;
        }
    }
}