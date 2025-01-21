using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class ComboBoxDropDownCornerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius cornerRadius)
            {
                return new CornerRadius(0,
                    cornerRadius.TopRight > 1 ? cornerRadius.TopRight - 1 : cornerRadius.TopRight,
                    cornerRadius.BottomRight > 1 ? cornerRadius.BottomRight - 1 : cornerRadius.BottomRight,
                    0);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}