using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches
{
    public class SolidColorBrushAttach
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.RegisterAttached("Color", typeof(SolidColorBrush), typeof(SolidColorBrushAttach), new PropertyMetadata(OnColorChanged));

        public static void SetColor(DependencyObject element, SolidColorBrush value)
        {
            element.SetValue(ColorProperty, value);
        }

        public static SolidColorBrush GetColor(DependencyObject element)
        {
            return (SolidColorBrush)element.GetValue(ColorProperty);
        }

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SolidColorBrush solidColorBrush && e.NewValue is SolidColorBrush newValue)
            {
                solidColorBrush.Color = newValue.Color;
            }
        }
    }
}