using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class ToolBarAttach
    {
        public static readonly DependencyProperty IsShowToolBarThumbProperty =
            DependencyProperty.RegisterAttached("IsShowToolBarThumb", typeof(bool), typeof(ToolBarAttach), new PropertyMetadata(true));

        public static bool GetIsShowToolBarThumb(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowToolBarThumbProperty);
        }

        public static void SetIsShowToolBarThumb(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowToolBarThumbProperty, value);
        }
    }
}