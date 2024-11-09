using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class DropDownMinWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                var minWidth = (double)values[0];
                var titlePlacement = (Dock)values[1];
                var titleWidth = (double)values[2];
                if (titlePlacement is Dock.Top or Dock.Bottom)
                {
                    return minWidth;
                }
                return Math.Max(0, minWidth - titleWidth);
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}