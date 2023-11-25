using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OhmStudio.UI.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is not SolidColorBrush solidColorBrush)
            {
                return default;
            }
            return solidColorBrush.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}