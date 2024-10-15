using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class ToolBarAttach
    {
        public static readonly DependencyProperty ShowToolBarThumbProperty =
            DependencyProperty.RegisterAttached("ShowToolBarThumb", typeof(bool), typeof(ToolBarAttach), new PropertyMetadata(true));

        public static bool GetShowToolBarThumb(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowToolBarThumbProperty);
        }

        public static void SetShowToolBarThumb(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowToolBarThumbProperty, value);
        }
    }
}