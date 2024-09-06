using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Styles.Custom
{
    /// <summary>
    /// TextBoxAttach.xaml 的交互逻辑。
    /// </summary>
    public partial class TextBoxAttach : ResourceDictionary
    {
        private void ContentControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var contentControl = (ContentControl)sender;
            if (!e.Handled && Equals(e.OriginalSource, contentControl))
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
                        var first = uIElement.FindChildrenOfType<UIElement>().FirstOrDefault(x => x.Focusable);
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