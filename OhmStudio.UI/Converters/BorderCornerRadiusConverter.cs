using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class BorderCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius cornerRadius)
            {
                return new CornerRadius(0, Math.Max(cornerRadius.TopRight - 1, 0), Math.Max(cornerRadius.BottomRight - 1, 0), 0);
            }
            return default(CornerRadius);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}