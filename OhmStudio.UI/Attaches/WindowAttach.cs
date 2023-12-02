using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class WindowAttach
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(WindowAttach), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is Window window && window.IsLoaded)
                {
                    window.DialogResult = (bool?)e.NewValue;
                }
            }));

        public static bool? GetDialogResult(DependencyObject obj)
        {
            return (bool?)obj.GetValue(DialogResultProperty);
        }

        public static void SetDialogResult(DependencyObject obj, bool? value)
        {
            obj.SetValue(DialogResultProperty, value);
        }
    }
}