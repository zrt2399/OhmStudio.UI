using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class DigitalCompensationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var result))
            {
                return result + System.Convert.ToDouble(parameter);
            }
            return default(double);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
