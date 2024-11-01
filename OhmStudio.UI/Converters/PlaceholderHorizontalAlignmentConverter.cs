using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class PlaceholderHorizontalAlignmentConverter : IValueConverter
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Control control)
            {
                return control.HorizontalContentAlignment;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}