using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using OhmStudio.UI.Converters;

namespace OhmStudio.UI.Attaches
{
    public class RectangleAttach
    {
        public static readonly DependencyProperty IsCircularProperty =
            DependencyProperty.RegisterAttached("IsCircular", typeof(bool), typeof(RectangleAttach), new PropertyMetadata(false, OnCircularChanged));

        private static void OnCircularChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Rectangle rectangle)
            {
                if ((bool)e.NewValue)
                {
                    MultiBinding multiBinding = new MultiBinding
                    {
                        Converter = new RectangleCircularConverter()
                    };
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name)
                    {
                        Source = rectangle
                    });
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name)
                    {
                        Source = rectangle
                    });
                    rectangle.SetBinding(Rectangle.RadiusXProperty, multiBinding);
                    rectangle.SetBinding(Rectangle.RadiusYProperty, multiBinding);
                }
                else
                {
                    BindingOperations.ClearBinding(rectangle, FrameworkElement.ActualWidthProperty);
                    BindingOperations.ClearBinding(rectangle, FrameworkElement.ActualHeightProperty);
                    BindingOperations.ClearBinding(rectangle, Rectangle.RadiusXProperty);
                    BindingOperations.ClearBinding(rectangle, Rectangle.RadiusYProperty);
                }
            }
        }

        public static void SetIsCircular(DependencyObject element, bool value)
        {
            element.SetValue(IsCircularProperty, value);
        }

        public static bool GetIsCircular(DependencyObject element)
        {
            return (bool)element.GetValue(IsCircularProperty);
        }
    }
}