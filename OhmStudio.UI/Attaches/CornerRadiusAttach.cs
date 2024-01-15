using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;

namespace OhmStudio.UI.Attaches
{
    public class CornerRadiusAttach
    {
        public static readonly DependencyProperty IsCircularProperty =
            DependencyProperty.RegisterAttached("IsCircular", typeof(bool), typeof(CornerRadiusAttach), new PropertyMetadata(OnIsCircularChanged));

        private static void OnIsCircularChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyProperty dependencyProperty = GetCornerRadiusProperty(sender);
            if (dependencyProperty != null)
            {
                FrameworkElement frameworkElement = sender as FrameworkElement;
                if ((bool)e.NewValue)
                {
                    MultiBinding multiBinding = new MultiBinding
                    {
                        Converter = new BorderCircularConverter()
                    };
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name)
                    {
                        Source = frameworkElement
                    });
                    multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name)
                    {
                        Source = frameworkElement
                    });
                    frameworkElement.SetBinding(dependencyProperty, multiBinding);
                }
                else
                {
                    BindingOperations.ClearBinding(frameworkElement, FrameworkElement.ActualWidthProperty);
                    BindingOperations.ClearBinding(frameworkElement, FrameworkElement.ActualHeightProperty);
                    BindingOperations.ClearBinding(frameworkElement, dependencyProperty);
                }
            }
        }

        public static DependencyProperty GetCornerRadiusProperty(DependencyObject element)
        {
            if (element is Border)
            {
                return Border.CornerRadiusProperty;
            }
            else if (element is DropShadowControl)
            {
                return DropShadowControl.CornerRadiusProperty;
            }
            else if (element is GifImage)
            {
                return GifImage.CornerRadiusProperty;
            }
            else
            {
                return BorderAttach.CornerRadiusProperty;
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