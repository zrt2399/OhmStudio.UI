using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace OhmStudio.UI.Converters
{
    public class CircleProgressBarMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                return new Thickness(Math.Max(System.Convert.ToDouble(values[0]), System.Convert.ToDouble(values[1])) / 2);
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}