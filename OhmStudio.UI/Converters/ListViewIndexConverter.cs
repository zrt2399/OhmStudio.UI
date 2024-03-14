using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class ListViewIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ListViewItem listViewItem)
            {
                return default;
            }
            ListView listView = ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView;
            return listView.ItemContainerGenerator.IndexFromContainer(listViewItem) + (parameter == null ? 1 : System.Convert.ToInt32(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}