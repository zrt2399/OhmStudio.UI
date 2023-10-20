using System;
using System.Globalization;
using System.Windows.Data;
using AvalonDock.Controls;
using AvalonDock.Layout;

namespace OhmStudio.UI.Converters
{
    public class TitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var floatingWindow = value as LayoutDocumentFloatingWindowControl;

            var mode = floatingWindow.Model as LayoutDocumentFloatingWindow;

            return mode.SinglePane.SelectedContent.Title;

            //return Application.Current?.MainWindow.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
