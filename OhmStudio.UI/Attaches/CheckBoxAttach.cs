using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class CheckBoxAttach
    {
        public static readonly DependencyProperty CheckBoxSizeProperty =
            DependencyProperty.RegisterAttached("CheckBoxSize", typeof(double), typeof(CheckBoxAttach));

        public static void SetCheckBoxSize(DependencyObject element, double value)
        {
            element.SetValue(CheckBoxSizeProperty, value);
        }

        public static double GetCheckBoxSize(DependencyObject element)
        {
            return (double)element.GetValue(CheckBoxSizeProperty);
        }
    }
}