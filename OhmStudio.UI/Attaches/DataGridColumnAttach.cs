using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class DataGridColumnAttach
    {
        public static readonly DependencyProperty IgnoreElementStyleProperty =
            DependencyProperty.RegisterAttached("IgnoreElementStyle", typeof(bool), typeof(DataGridColumnAttach));

        public static bool GetIgnoreElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IgnoreElementStyleProperty);
        }

        public static void SetIgnoreElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IgnoreElementStyleProperty, value);
        }

        public static readonly DependencyProperty IgnoreEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("IgnoreEditingElementStyle", typeof(bool), typeof(DataGridColumnAttach));

        public static bool GetIgnoreEditingElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IgnoreEditingElementStyleProperty);
        }

        public static void SetIgnoreEditingElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IgnoreEditingElementStyleProperty, value);
        }
    }
}