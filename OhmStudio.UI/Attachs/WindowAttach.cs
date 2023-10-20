using System.Windows;

namespace OhmStudio.UI.Attachs
{
    public class WindowAttach
    {
        public static readonly DependencyProperty ShowDialogProperty =
           DependencyProperty.RegisterAttached("ShowDialog", typeof(bool?), typeof(TextBoxAttach), new PropertyMetadata(null, (sender, e) =>
           {
               if (sender is Window window && window.IsLoaded)
               {
                   window.DialogResult = (bool?)e.NewValue;
               }
           }));

        public static bool? GetShowDialog(DependencyObject target)
        {
            return (bool?)target.GetValue(ShowDialogProperty);
        }

        public static void SetShowDialog(DependencyObject target, bool? value)
        {
            target.SetValue(ShowDialogProperty, value);
        }
    }
}