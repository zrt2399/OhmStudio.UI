using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using OhmStudio.UI.PublicMethod;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width - 200;
            if (AlertDialog.UILanguage == UILanguage.Zh_TW)
            {
                btnOK.Content = "確定";
                btnCancel.Content = "取消";
            }
            else if (AlertDialog.UILanguage == UILanguage.En_US)
            {
                btnOK.Content = "OK";
                btnCancel.Content = "Cancel";
            } 
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (SizeToContent == SizeToContent.WidthAndHeight && WindowChrome.GetWindowChrome(this) != null)
            {
                InvalidateMeasure();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var foreground = Application.Current?.Resources["CommonControlsTextBoxTextDisabled"] as Brush;
            txtTitle.Foreground = foreground;
            btnClose.Foreground = foreground;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var foreground = Application.Current?.Resources["CommonControlsTextBoxText"] as Brush;
            txtTitle.Foreground = foreground;
            btnClose.Foreground = foreground;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            imageInfo.Source = null; 
        }
    }
}