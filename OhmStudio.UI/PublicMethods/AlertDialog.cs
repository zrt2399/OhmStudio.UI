using System;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.PublicMethods
{
    /// <summary>
    /// 表示当前语言。
    /// </summary>
    public enum OhmUILanguage
    {
        Zh_CN,
        Zh_TW,
        En_US
    }

    /// <summary>
    /// 表示窗口显示的按钮。
    /// </summary>
    public enum MessageButton
    {
        OK,
        OKCancel
    }

    /// <summary>
    /// 表示窗口显示的图片。
    /// </summary>
    public enum MessageImage
    {
        None,
        Information,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// 表示一个消息提示框类。
    /// </summary>
    public static class AlertDialog
    {
        public static OhmUILanguage OhmUILanguage { get; set; } = OhmUILanguage.Zh_CN;
        public static bool Show(string message, string title = null, MessageButton button = MessageButton.OK, MessageImage image = MessageImage.Information, Window owner = null)
        {
            bool flag = false;
            try
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    MessageWindow messageWindow = new MessageWindow();
                    messageWindow.SetOwner(owner);
                    messageWindow.Title = title ?? GetTitle();
                    messageWindow.txtMessage.Text = message;
                    messageWindow.btnCancel.Visibility = button == MessageButton.OK ? Visibility.Collapsed : Visibility.Visible;
                    messageWindow.imageInfo.Source = GetImage(image);
                    flag = messageWindow.ShowDialog() == true;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return flag;
        }

        static readonly BitmapSource _error = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        static readonly BitmapSource _information = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        static readonly BitmapSource _warning = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        static readonly BitmapSource _question = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        private static BitmapSource GetImage(MessageImage messageImage)
        {
            switch (messageImage)
            {
                case MessageImage.None:
                    return null;
                case MessageImage.Information:
                    SystemSounds.Asterisk.Play();
                    return _information;
                case MessageImage.Warning:
                    SystemSounds.Exclamation.Play();
                    return _warning;
                case MessageImage.Question:
                    SystemSounds.Question.Play();
                    return _question;
                default:
                    SystemSounds.Hand.Play();
                    return _error;
            }
            //return new BitmapImage(new Uri("pack://application:,,,/OhmStudio.UI;component/Images/" + urlString + ".png"));
        }

        public static bool ShowWarning(string message, string title = "警告")
        {
            if (OhmUILanguage == OhmUILanguage.Zh_TW)
            {
                title = "警告";
            }
            else if (OhmUILanguage == OhmUILanguage.En_US)
            {
                title = "Warning";
            }
            return Show(message, title, MessageButton.OK, MessageImage.Warning);
        }

        public static bool ShowError(string message, string title = "出错了")
        {
            if (OhmUILanguage == OhmUILanguage.Zh_TW)
            {
                title = "出錯了";
            }
            else if (OhmUILanguage == OhmUILanguage.En_US)
            {
                title = "Error";
            }
            return Show(message, title, MessageButton.OK, MessageImage.Error);
        }

        //static bool SetWindowStartupLocation(MessageWindow messageWindow)
        //{
        //    //    if (Windows.Count < 1)
        //    //    {
        //    //        Windows.Add(messageWindow);
        //    //        return messageWindow.ShowDialog() == true;
        //    //    }
        //    //    var window = Windows.LastOrDefault();
        //    //var window = Windows.Where(x => x.Owner == messageWindow.Owner).LastOrDefault();
        //    //if (window == null)
        //    //{
        //    //    Windows.Add(messageWindow);
        //    //    return messageWindow.ShowDialog() == true;
        //    //}
        //    //Windows.Add(messageWindow);
        //    messageWindow.Top = window.Top + 40;
        //    messageWindow.Left = window.Left + 40;
        //    if (messageWindow.Top > SystemParameters.WorkArea.Height - 50 || messageWindow.Left > SystemParameters.WorkArea.Width - 200)
        //    {
        //        messageWindow.Top = messageWindow.Left = 0;
        //    }
        //    return messageWindow.ShowDialog() == true;
        //}

        private static string GetTitle()
        {
            return OhmUILanguage switch
            {
                OhmUILanguage.Zh_CN => "系统提示",
                OhmUILanguage.Zh_TW => "系統提示",
                _ => "System prompt"
            };
        }
    }
}