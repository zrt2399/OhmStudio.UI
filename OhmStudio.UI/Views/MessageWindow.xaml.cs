using System;
using System.Windows;
using System.Windows.Shell;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑。
    /// </summary>
    public partial class MessageWindow : ChromeWindow
    {
        public MessageWindow(MessageBoxButton messageBoxButton)
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
            if (messageBoxButton == MessageBoxButton.OK)
            {
                btnOK.Visibility = Visibility.Visible;
            }
            else if (messageBoxButton == MessageBoxButton.OKCancel)
            {
                btnOK.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
            }
            else if (messageBoxButton == MessageBoxButton.YesNoCancel)
            {
                btnYes.Visibility = Visibility.Visible;
                btnNo.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
            }
            else if (messageBoxButton == MessageBoxButton.YesNo)
            {
                btnYes.Visibility = Visibility.Visible;
                btnNo.Visibility = Visibility.Visible;
            }
        }

        public MessageBoxResult MessageBoxResult { get; private set; }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (SizeToContent != SizeToContent.Manual && WindowChrome.GetWindowChrome(this) != null)
            {
                InvalidateMeasure();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            imageInfo.Source = null;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.OK;
            DialogResult = true;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.No;
            DialogResult = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.Cancel;
            DialogResult = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}