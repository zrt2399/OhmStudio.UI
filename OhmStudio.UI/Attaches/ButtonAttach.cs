using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches
{
    public class ButtonAttach
    {
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.RegisterAttached("PressedBackground", typeof(Brush), typeof(ButtonAttach));

        public static void SetPressedBackground(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBackgroundProperty, value);
        }

        public static Brush GetPressedBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBackgroundProperty);
        }

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.RegisterAttached("PressedBorderBrush", typeof(Brush), typeof(ButtonAttach));

        public static void SetPressedBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBorderBrushProperty, value);
        }

        public static Brush GetPressedBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBorderBrushProperty);
        }
    }
} 