using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value?.ToString(), out bool result))
            {
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value == null)
            {
                if (parameter is Visibility visibilityParam)
                {
                    return visibilityParam;
                }
                return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}