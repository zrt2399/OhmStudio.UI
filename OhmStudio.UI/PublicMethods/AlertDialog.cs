﻿using System;
using System.Drawing;
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
    public enum LanguageType
    {
        Zh_CNS,
        Zh_CHT,
        En_US
    }

    /// <summary>
    /// 表示一个消息提示框类。
    /// </summary>
    public static class AlertDialog
    {
        public static LanguageType Language { get; set; } = LanguageType.Zh_CNS;

        public static MessageBoxResult Show(string message, string title = null, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage messageBoxImage = MessageBoxImage.Information, Window owner = null)
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.None;
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    MessageWindow messageWindow = new MessageWindow(messageBoxButton);
                    messageWindow.SetOwner(owner);
                    messageWindow.Title = title ?? GetTitle();
                    messageWindow.txtMessage.Text = message;
                    if (messageBoxButton == MessageBoxButton.OK)
                    {
                        messageWindow.btnCancel.Visibility = Visibility.Collapsed;
                    }
                    else if (messageBoxButton == MessageBoxButton.YesNoCancel)
                    {
                        messageWindow.btnNo.Visibility = Visibility.Visible;
                    }
                    if (messageBoxButton is MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel)
                    {
                        messageWindow.btnOK.Content = Language == LanguageType.En_US ? "Yes" : "是";
                    }

                    messageWindow.imageInfo.Source = GetImage(messageBoxImage);
                    messageWindow.ShowDialog();
                    messageBoxResult = messageWindow.MessageBoxResult;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return messageBoxResult;
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
            if (Language == LanguageType.Zh_CHT)
            {
                title = "警告";
            }
            else if (Language == LanguageType.En_US)
            {
                title = "Warning";
            }
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK;
        }

        public static bool ShowError(string message, string title = "错误")
        {
            if (Language == LanguageType.Zh_CHT)
            {
                title = "錯誤";
            }
            else if (Language == LanguageType.En_US)
            {
                title = "Error";
            }
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK;
        }

        private static string GetTitle()
        {
            return Language switch
            {
                LanguageType.Zh_CNS => "系统提示",
                LanguageType.Zh_CHT => "系統提示",
                _ => "System prompt"
            };
        }
    }
}