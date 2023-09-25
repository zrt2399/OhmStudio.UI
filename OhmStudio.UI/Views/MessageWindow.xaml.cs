using System;
using System.Linq;
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
        public MessageWindow(int id)
        {
            InitializeComponent();
            Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            Id = id;
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width - 200;
            if (MessageTip.UILanguage == UILanguage.zh_TW)
            {
                btnOK.Content = "確定";
                btnCancel.Content = "取消";
            }
            else if (MessageTip.UILanguage == UILanguage.en_US)
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

        public int Id { get; set; }
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
            txtTitle.Foreground = Brushes.LightGray;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtTitle.Foreground = Brushes.White;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            imageInfo.Source = null;
            MessageTip.windows.TryRemove(Id, out _);
        }
    }
}