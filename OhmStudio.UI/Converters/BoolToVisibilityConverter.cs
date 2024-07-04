using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool result && result)
            {
                return Visibility.Visible;
            }
            if (parameter is Visibility visibilityParam)
            {
                return visibilityParam;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool result || !result)
            {
                return Visibility.Visible;
            }
            if (parameter is Visibility visibilityParam)
            {
                return visibilityParam;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}