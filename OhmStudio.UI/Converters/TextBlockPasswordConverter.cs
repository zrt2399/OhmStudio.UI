using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class TextBlockPasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new string('●', value.ToString().Length);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new string('●', value.ToString().Length);
            }
            return string.Empty;
        }
    }
}
