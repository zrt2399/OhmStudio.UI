using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class AngleToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double result)
            {
                double radius = 50;
                var piang = result * Math.PI / 180;

                var px = Math.Sin(piang) * radius + radius;
                var py = -Math.Cos(piang) * radius + radius;
                return new Point(px, py);
            }
            else
            {
                return default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}