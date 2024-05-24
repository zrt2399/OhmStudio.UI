using System;
using System.Globalization;
using System.Windows.Data;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Converters
{
    public class FlipToScaleXValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconFlipOrientation flip)
            {
                var scaleX = flip == PackIconFlipOrientation.Horizontal || flip == PackIconFlipOrientation.Both ? -1 : 1;
                return scaleX;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FlipToScaleYValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconFlipOrientation flip)
            {
                var scaleY = flip == PackIconFlipOrientation.Vertical || flip == PackIconFlipOrientation.Both ? -1 : 1;
                return scaleY;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 