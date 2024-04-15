using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public class RadioButtonAttach
    {
        public static readonly DependencyProperty RadioButtonSizeProperty =
            DependencyProperty.RegisterAttached("RadioButtonSize", typeof(double), typeof(RadioButtonAttach));

        public static void SetRadioButtonSize(DependencyObject element, double value)
        {
            element.SetValue(RadioButtonSizeProperty, value);
        }

        public static double GetRadioButtonSize(DependencyObject element)
        {
            return (double)element.GetValue(RadioButtonSizeProperty);
        }
    }
} 