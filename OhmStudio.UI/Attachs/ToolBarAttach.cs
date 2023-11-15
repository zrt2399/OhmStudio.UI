using System.Windows;

namespace OhmStudio.UI.Attachs
{
    public class ToolBarAttach
    {
        public static readonly DependencyProperty ToolBarThumbVisibilityProperty =
            DependencyProperty.RegisterAttached("ToolBarThumbVisibility", typeof(Visibility), typeof(ToolBarAttach), new PropertyMetadata(Visibility.Visible));

        public static Visibility GetToolBarThumbVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(ToolBarThumbVisibilityProperty);
        }

        public static void SetToolBarThumbVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(ToolBarThumbVisibilityProperty, value);
        }
    }
}