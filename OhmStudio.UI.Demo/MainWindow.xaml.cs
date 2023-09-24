using System.Windows;
using OhmStudio.UI.Controls;
using OhmStudio.UI.PublicMethod;

namespace OhmStudio.UI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageTip.Show("123");
        }
    }
}