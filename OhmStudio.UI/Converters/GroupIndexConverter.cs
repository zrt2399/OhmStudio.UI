using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class GroupIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && values[0] is IList list)
            {
                var id = list.IndexOf(values[1]) + (parameter == null ? 1 : System.Convert.ToInt32(parameter));
                return id.ToString();
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}