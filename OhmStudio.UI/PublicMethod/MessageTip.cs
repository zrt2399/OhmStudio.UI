using System.Collections.Concurrent;
using OhmStudio.UI.Views;
using System.Windows;
using System.Windows.Media.Imaging;
using System;

namespace OhmStudio.UI.PublicMethod
{
    /// <summary>
    /// 表示当前语言。
    /// </summary>
    public enum UILanguage
    {
        zh_CN,
        zh_TW,
        en_US
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
    public class MessageTip
    {
        public static ConcurrentDictionary<int, Window> windows = new ConcurrentDictionary<int, Window>();
        public static UILanguage UILanguage { get; set; } = UILanguage.zh_CN;
        public static bool Show(string message, string title = null, MyStyle myStyle = MyStyle.Blue, MessageButton button = MessageButton.OK, MessageImage image = MessageImage.Info)
        {
            bool flag = false;
            try
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow messageWindow = new MessageWindow(windows.Count);
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
                            goto IL_F6;
                        case MessageImage.Warning:
                            bitmapImage = GetImage("warning");
                            goto IL_F6;
                        case MessageImage.Question:
                            bitmapImage = GetImage("question");
                            goto IL_F6;
                    }
                    bitmapImage = GetImage("error");
                IL_F6:
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

        public static void ShowWarning(string message, string title = "警告")
        {
            if (UILanguage == UILanguage.zh_TW)
            {
                title = "警告";
            }
            else if (UILanguage == UILanguage.en_US)
            {
                title = "Warning";
            }
            ShowWindow(message, "#FFDC9B28", "警告", title, MessageButton.OK);
        }

        public static void ShowError(string message, string title = "出错了")
        {
            if (UILanguage == UILanguage.zh_TW)
            {
                title = "出錯了";
            }
            else if (UILanguage == UILanguage.en_US)
            {
                title = "Error";
            }
            ShowWindow(message, "#FFF03A17", "错误", title, MessageButton.OK);
        }

        static bool SetWindowStartupLocation(MessageWindow messageWindow)
        {
            if (windows.Count < 1)
            {
                messageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                windows.TryAdd(messageWindow.Id, messageWindow);
                return messageWindow.ShowDialog() == true;
            }
            var window = windows[windows.Count - 1];
            windows.TryAdd(messageWindow.Id, messageWindow);
            messageWindow.Top = window.Top + 40;
            messageWindow.Left = window.Left + 40;
            if (messageWindow.Top > SystemParameters.WorkArea.Height - 50 || messageWindow.Left > SystemParameters.WorkArea.Width - 200)
            {
                messageWindow.Top = messageWindow.Left = 0;
            }
            return messageWindow.ShowDialog() == true;
        }

        private static void ShowWindow(string message, string brushString, string url, string title = null, MessageButton button = MessageButton.OK)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MessageWindow messageWindow = new MessageWindow(windows.Count);
                    messageWindow.txtTitle.Text = messageWindow.Title = title ?? GetTitle();
                    messageWindow.title.Background = brushString.ToSolidColorBrush();
                    messageWindow.txtMessage.Text = message;
                    messageWindow.btnCancel.Visibility = button == MessageButton.OK ? Visibility.Collapsed : Visibility.Visible;
                    messageWindow.imageInfo.Source = GetImage(url);
                    SetWindowStartupLocation(messageWindow);
                    //messageWindow.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string GetTitle()
        {
            return UILanguage switch
            {
                UILanguage.zh_CN => "系统提示",
                UILanguage.zh_TW => "系統提示",
                _ => "System prompt"
            };
        }
    }
}