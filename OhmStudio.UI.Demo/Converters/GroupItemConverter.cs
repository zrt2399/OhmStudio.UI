using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using OhmStudio.UI.Demo.Views;

namespace OhmStudio.UI.Demo.Converters
{
    public class GroupItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as ReadOnlyObservableCollection<object>;
            if (type.Count > 0)
            {
                return (type[0] as Employee).IsExpanded;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}