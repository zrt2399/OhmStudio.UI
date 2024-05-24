using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    public class IconButton : Button
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(IconButton));

        public ImageSource Icon
        {
            set => SetValue(IconProperty, value);
            get => (ImageSource)GetValue(IconProperty);
        }
    }
} 