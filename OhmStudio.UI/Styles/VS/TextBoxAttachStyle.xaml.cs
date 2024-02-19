using System.Windows;
using System.Windows.Controls;

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
                        uIElement.Focus();
                        e.Handled = true;
                    }
                }
            }
        }
    }
}