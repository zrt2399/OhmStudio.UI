using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OhmStudio.UI.Converters;

namespace OhmStudio.UI.Attaches
{
    public class BorderAttach
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(BorderAttach), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty IsCircularProperty =
            DependencyProperty.RegisterAttached("IsCircular", typeof(bool), typeof(BorderAttach), new PropertyMetadata(false, OnIsCircularChanged));

        public static void SetCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        public static CornerRadius GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        private static void OnIsCircularChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Border border)
            {
                if ((bool)e.NewValue)
                {
                    MultiBinding multiBinding = new MultiBinding
                    {
                        Converter = new BorderCircularConverter()
                    };
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name)
                    {
                        Source = border
                    });
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name)
                    {
                        Source = border
                    });
                    border.SetBinding(Border.CornerRadiusProperty, multiBinding);
                }
                else
                {
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualWidthProperty);
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualHeightProperty);
                    BindingOperations.ClearBinding(border, Border.CornerRadiusProperty);
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