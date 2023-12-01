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

        public static readonly DependencyProperty ToolBarThumbMarginProperty =
            DependencyProperty.RegisterAttached("ToolBarThumbMargin", typeof(Thickness), typeof(ToolBarAttach), new PropertyMetadata(new Thickness(0, 2, 0, 2)));
        public static Thickness GetToolBarThumbMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ToolBarThumbMarginProperty);
        }

        public static void SetToolBarThumbMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ToolBarThumbMarginProperty, value);
        }
    }
}