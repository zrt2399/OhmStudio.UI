using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class ExpanderRotateAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = 1.0;
            if (double.TryParse(parameter?.ToString(), out var result))
            {
                factor = result;
            }
            return value switch
            {
                ExpandDirection.Left => 90 * factor,
                ExpandDirection.Right => -90 * factor,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}