using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class DocumentWrapPanelAttach
    {
        public static readonly DependencyProperty IsWrapProperty =
            DependencyProperty.RegisterAttached("IsWrap", typeof(bool), typeof(DocumentWrapPanelAttach),
                new FrameworkPropertyMetadata(false));

        public static void SetIsWrap(DependencyObject element, bool value)
        {
            element.SetValue(IsWrapProperty, value);
        }

        public static bool GetIsWrap(DependencyObject element)
        {
            return (bool)element.GetValue(IsWrapProperty);
        }

        public static readonly DependencyProperty IsMouseWheelWrapProperty =
           DependencyProperty.RegisterAttached("IsMouseWheelWrap", typeof(bool), typeof(DocumentWrapPanelAttach),
               new FrameworkPropertyMetadata(false));

        public static void SetIsMouseWheelWrap(DependencyObject element, bool value)
        {
            element.SetValue(IsMouseWheelWrapProperty, value);
        }

        public static bool GetIsMouseWheelWrap(DependencyObject element)
        {
            return (bool)element.GetValue(IsMouseWheelWrapProperty);
        }
    }
}