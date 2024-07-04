using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            if (parameter is Visibility visibilityParam)
            {
                return visibilityParam == Visibility.Visible;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class CollapsedToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible; 
            }
            if (parameter is Visibility visibilityParam)
            {
                return visibilityParam != Visibility.Visible;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}