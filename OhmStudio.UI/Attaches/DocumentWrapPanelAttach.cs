using System.Windows;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Attaches
{
    public class DocumentWrapPanelAttach
    {
        public static readonly DependencyProperty IsWrapProperty =
            DependencyProperty.RegisterAttached("IsWrap", typeof(bool), typeof(DocumentWrapPanelAttach),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, (sender, e) =>
                {
                    if (sender is DocumentWrapPanel documentWrapPanel)
                    {
                        documentWrapPanel.IsWrap = (bool)e.NewValue;
                    }
                }));

        public static void SetIsWrap(DependencyObject element, bool value)
        {
            element.SetValue(IsWrapProperty, value);
        }

        public static bool GetIsWrap(DependencyObject element)
        {
            return (bool)element.GetValue(IsWrapProperty);
        }
    }
}