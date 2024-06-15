using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class PathPickerPlaceHolderMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                var borderThickness = (Thickness)values[0];
                var padding = (Thickness)values[1];
                return new Thickness(borderThickness.Left + padding.Left, borderThickness.Top + padding.Top, borderThickness.Right + padding.Right, borderThickness.Bottom + padding.Bottom);
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}