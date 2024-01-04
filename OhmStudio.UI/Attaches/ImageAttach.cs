using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches
{
    public class ImageAttach
    {
        public static readonly DependencyProperty SourceFailedProperty =
            DependencyProperty.RegisterAttached("SourceFailed", typeof(ImageSource), typeof(ImageAttach), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnSourceFailedChanged));

        private static void OnSourceFailedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Image image)
            {
                if (e.NewValue is ImageSource)
                {
                    image.ImageFailed += Image_ImageFailed;
                }
                else
                {
                    image.ImageFailed -= Image_ImageFailed;
                }
            }
        }

        private static void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image image)
            {
                image.SetCurrentValue(Image.SourceProperty, GetSourceFailed(image));
                image.ImageFailed -= Image_ImageFailed;
            }
        }

        public static void SetSourceFailed(DependencyObject element, ImageSource value)
        {
            element.SetValue(SourceFailedProperty, value);
        }

        public static ImageSource GetSourceFailed(DependencyObject element)
        {
            return (ImageSource)element.GetValue(SourceFailedProperty);
        }
    }
} 