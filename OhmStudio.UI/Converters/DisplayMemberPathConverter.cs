using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class DisplayMemberPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object path;
            string displayMemberPath = values[0]?.ToString();
            string value = values[1]?.ToString();
            if (string.IsNullOrWhiteSpace(displayMemberPath))
            {
                path = value;
            }
            else
            {
                path = values[1].GetType().GetProperty(displayMemberPath)?.GetValue(values[1]);
            }
            return path;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
