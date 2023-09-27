using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.PublicMethod
{
    /// <summary>
    /// 表示当前语言。
    /// </summary>
    public enum UILanguage
    {
        Zh_CN,
        Zh_TW,
        En_US
    }

    /// <summary>
    /// 表示窗体显示的按钮。
    /// </summary>
    public enum MessageButton
    {
        OK,
        OKCancel
    }

    /// <summary>
    /// 表示窗体显示的图片。
    /// </summary>
    public enum MessageImage
    {
        Info,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// 表示窗体风格。
    /// </summary>
    public enum MyStyle
    {
        Blue,
        Yellow,
        Red
    }

    /// <summary>
    /// 表示一个对话框类。
    /// </summary>
    public static class MessageFrame
    {
        internal static List<Window> Windows = new List<Window>();
        public static UILanguage UILanguage { get; set; } = UILanguage.Zh_CN;
        public static bool Show(string message, string title = null, MyStyle myStyle = MyStyle.Blue, MessageButton button = MessageButton.OK, MessageImage image = MessageImage.Info)
        {
            bool flag = false;
            try
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    MessageWindow messageWindow = new MessageWindow();
                    messageWindow.txtTitle.Text = messageWindow.Title = title ?? GetTitle();
                    messageWindow.txtMessage.Text = message;
                    messageWindow.btnCancel.Visibility = (button == MessageButton.OK) ? Visibility.Collapsed : Visibility.Visible;
                    messageWindow.title.Background = myStyle switch
                    {
                        MyStyle.Blue => "#FF0078D7".ToSolidColorBrush(),
                        MyStyle.Yellow => "#FFDC9B28".ToSolidColorBrush(),
                        _ => "#FFF03A17".ToSolidColorBrush()
                    };
                    BitmapImage bitmapImage;
                    switch (image)
                    {
                        case MessageImage.Info:
                            bitmapImage = GetImage("info");
                            SystemSounds.Asterisk.Play();   
                            break;
                        case MessageImage.Warning:
                            bitmapImage = GetImage("warning");
                            SystemSounds.Exclamation.Play();
                            break;
                        case MessageImage.Question:
                            bitmapImage = GetImage("question");
                            SystemSounds.Question.Play();
                            break;
                        default:
                            bitmapImage = GetImage("error");
                            SystemSounds.Hand.Play();
                            break;
                    }
                    messageWindow.imageInfo.Source = bitmapImage;
                    flag = SetWindowStartupLocation(messageWindow);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return flag;
        }

        private static BitmapImage GetImage(string urlString)
        {
            return new BitmapImage(new Uri("pack://application:,,,/OhmStudio.UI;component/Images/" + urlString + ".png"));
        }

        public static bool ShowWarning(string message, string title = "警告")
        {
            if (UILanguage == UILanguage.Zh_TW)
            {
                title = "警告";
            }
            else if (UILanguage == UILanguage.En_US)
            {
                title = "Warning";
            }
            return Show(message, title, MyStyle.Yellow, MessageButton.OK, MessageImage.Warning);
        }

        public static bool ShowError(string message, string title = "出错了")
        {
            if (UILanguage == UILanguage.Zh_TW)
            {
                title = "出錯了";
            }
            else if (UILanguage == UILanguage.En_US)
            {
                title = "Error";
            }
            return Show(message, title, MyStyle.Red, MessageButton.OK, MessageImage.Error);
        }

        static bool SetWindowStartupLocation(MessageWindow messageWindow)
        {
            if (Windows.Count < 1)
            {
                Windows.Add(messageWindow);
                return messageWindow.ShowDialog() == true;
            }
            var window = Windows.Where(x => x.Owner == messageWindow.Owner).LastOrDefault();
            if (window == null)
            {
                Windows.Add(messageWindow);
                return messageWindow.ShowDialog() == true;
            }
            Windows.Add(messageWindow);
            messageWindow.Top = window.Top + 40;
            messageWindow.Left = window.Left + 40;
            if (messageWindow.Top > SystemParameters.WorkArea.Height - 50 || messageWindow.Left > SystemParameters.WorkArea.Width - 200)
            {
                messageWindow.Top = messageWindow.Left = 0;
            }
            return messageWindow.ShowDialog() == true;
        }

        //private static void ShowWindow(string message, string brushString, string url, string title = null, MessageButton button = MessageButton.OK)
        //{
        //    try
        //    {
        //        Application.Current?.Dispatcher.Invoke(delegate
        //        {
        //            MessageWindow messageWindow = new MessageWindow();
        //            messageWindow.txtTitle.Text = messageWindow.Title = title ?? GetTitle();
        //            messageWindow.title.Background = brushString.ToSolidColorBrush();
        //            messageWindow.txtMessage.Text = message;
        //            messageWindow.btnCancel.Visibility = button == MessageButton.OK ? Visibility.Collapsed : Visibility.Visible;
        //            messageWindow.imageInfo.Source = GetImage(url);
        //            SetWindowStartupLocation(messageWindow);
        //            //messageWindow.ShowDialog();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private static string GetTitle()
        {
            return UILanguage switch
            {
                UILanguage.Zh_CN => "系统提示",
                UILanguage.Zh_TW => "系統提示",
                _ => "System prompt"
            };
        }
    }
}