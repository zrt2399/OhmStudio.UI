using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    [ValueConversion(typeof(double), typeof(bool))]
    public class AngleToIsLargeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double result && result > 180;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}