using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class InfinityToMaxOrMinValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                if (double.IsPositiveInfinity(doubleValue))
                {
                    return double.MaxValue;
                }
                else if (double.IsNegativeInfinity(doubleValue))
                {
                    return double.MinValue;
                }
                return doubleValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}