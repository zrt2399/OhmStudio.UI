using System;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class DisplayMemberPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length > 1)
            {
                string value = values[0]?.ToString();
                string displayMemberPath = values[1]?.ToString();
                if (string.IsNullOrWhiteSpace(displayMemberPath))
                {
                    return value;
                }
                else
                {
                    return values[0].GetType().GetProperty(displayMemberPath)?.GetValue(values[0]);
                }
            }
            return default;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}