using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class DataGridColumnAttach
    {
        public static readonly DependencyProperty IsIgnoreElementStyleProperty =
            DependencyProperty.RegisterAttached("IsIgnoreElementStyle", typeof(bool), typeof(DataGridColumnAttach));

        public static bool GetIsIgnoreElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IsIgnoreElementStyleProperty);
        }

        public static void SetIsIgnoreElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IsIgnoreElementStyleProperty, value);
        }

        public static readonly DependencyProperty IsIgnoreEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("IsIgnoreEditingElementStyle", typeof(bool), typeof(DataGridColumnAttach));

        public static bool GetIsIgnoreEditingElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IsIgnoreEditingElementStyleProperty);
        }

        public static void SetIsIgnoreEditingElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IsIgnoreEditingElementStyleProperty, value);
        }
    }
}