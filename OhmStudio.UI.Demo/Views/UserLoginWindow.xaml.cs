using System;
using System.Windows;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Controls;

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
    }
}