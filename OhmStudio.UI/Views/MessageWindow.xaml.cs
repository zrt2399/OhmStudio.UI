using System;
using System.Windows;
using System.Windows.Shell;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑。
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(MessageBoxButton messageBoxButton)
        {
            InitializeComponent();
            _messageBoxButton = messageBoxButton;
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
            if (AlertDialog.Language == LanguageType.Zh_CHT)
            {
                btnOK.Content = "確定";
                btnNo.Content = "否";
                btnCancel.Content = "取消";
            }
            else if (AlertDialog.Language == LanguageType.En_US)
            {
                btnOK.Content = "OK";
                btnNo.Content = "No";
                btnCancel.Content = "Cancel";
            }
        }

        readonly MessageBoxButton _messageBoxButton;

        public MessageBoxResult MessageBoxResult { get; set; }

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
            MessageBoxResult = _messageBoxButton is MessageBoxButton.OK or MessageBoxButton.OKCancel ? MessageBoxResult.OK : MessageBoxResult.Yes;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            imageInfo.Source = null;
        }
    }
}