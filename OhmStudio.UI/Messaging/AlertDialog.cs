using System;
using System.Drawing;
using System.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using OhmStudio.UI.Utilities;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.Messaging
{
    /// <summary>
    /// 表示一个消息提示框类。
    /// </summary>
    public static class AlertDialog
    {
        public static MessageBoxResult Show(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage messageBoxImage = MessageBoxImage.Information, Window owner = null)
        {
            if (Application.Current is not Application application || application.Dispatcher == null)
            {
                return MessageBoxResult.None;
            }

            if (application.Dispatcher.CheckAccess())
            {
                MessageBoxResult messageBoxResult = MessageBoxResult.None;
                try
                {
                    MessageWindow messageWindow = new MessageWindow(messageBoxButton);
                    messageWindow.SetOwner(owner);
                    messageWindow.Title = title ?? string.Empty;
                    messageWindow.txtMessage.Text = message;
                    messageWindow.imageInfo.Source = GetImage(messageBoxImage);
                    messageWindow.ShowDialog();
                    messageBoxResult = messageWindow.MessageBoxResult;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return messageBoxResult;
            }
            else
            {
                return application.Dispatcher.Invoke(() => Show(message, title, messageBoxButton, messageBoxImage, owner));
            }
        }

        private static BitmapSource GetImage(MessageBoxImage messageBoxImage)
        {
            switch (messageBoxImage)
            {
                case MessageBoxImage.None:
                    return null;
                case MessageBoxImage.Information or MessageBoxImage.Asterisk:
                    SystemSounds.Asterisk.Play();
                    return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                case MessageBoxImage.Warning or MessageBoxImage.Exclamation:
                    SystemSounds.Exclamation.Play();
                    return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                case MessageBoxImage.Question:
                    SystemSounds.Question.Play();
                    return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                default:
                    SystemSounds.Hand.Play();
                    return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            //return new BitmapImage(new Uri("pack://application:,,,/OhmStudio.UI;component/Images/" + urlString + ".png"));
        }

        public static bool ShowWarning(string message, string title = "警告")
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK;
        }

        public static bool ShowError(string message, string title = "错误")
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK;
        }
    }
}