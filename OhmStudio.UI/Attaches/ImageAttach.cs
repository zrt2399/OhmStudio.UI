using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OhmStudio.UI.Attaches
{
    public class ImageAttach
    {
        public static readonly DependencyProperty SourceFailedProperty =
            DependencyProperty.RegisterAttached("SourceFailed", typeof(ImageSource), typeof(ImageAttach), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnSourceFailedChanged));

        public static void SetSourceFailed(DependencyObject element, ImageSource value)
        {
            element.SetValue(SourceFailedProperty, value);
        }

        public static ImageSource GetSourceFailed(DependencyObject element)
        {
            return (ImageSource)element.GetValue(SourceFailedProperty);
        }

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

        public static readonly DependencyProperty SourceProperty =
           DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(ImageAttach), new PropertyMetadata((sender, e) =>
           {
               if (sender is Image image && e.NewValue is Uri uri)
               {
                   image.Source = null;
                   var bitmapImage = new BitmapImage();
                   //bitmapImage.CreateOptions = BitmapCreateOptions.None;
                   bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                   //bitmapImage.DecodePixelWidth = 100;
                   bitmapImage.BeginInit();
                   bitmapImage.UriSource = uri;
                   bitmapImage.EndInit();
                   //bitmapImage.Freeze();
                   image.Source = bitmapImage;
               }
           }));

        public static void SetSource(DependencyObject element, Uri value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static Uri GetSource(DependencyObject element)
        {
            return (Uri)element.GetValue(SourceProperty);
        }
    }
}