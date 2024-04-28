﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Styles.VS
{
    /// <summary>
    /// TextBoxAttachStyle.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxAttachStyle : ResourceDictionary
    {
        private void ContentControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var contentControl = (ContentControl)sender;
            if (!e.Handled)
            {
                if (Equals(e.OriginalSource, contentControl))
                {
                    if (contentControl.Content is UIElement uIElement)
                    {
                        if (uIElement.Focusable)
                        {
                            uIElement.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            var first = uIElement.FindChildren<UIElement>().FirstOrDefault(x => x.Focusable);
                            if (first != null)
                            {
                                first.Focus();
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}