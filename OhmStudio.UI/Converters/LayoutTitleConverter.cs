using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AvalonDock.Controls;
using AvalonDock.Layout;

namespace OhmStudio.UI.Converters
{
    public class LayoutTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var floatingWindow = value as LayoutFloatingWindowControl;

            var app = Application.Current?.MainWindow.Title + " - ";
            if (floatingWindow.Model is LayoutDocumentFloatingWindow layoutDocumentFloatingWindow)
            {
                return app + layoutDocumentFloatingWindow.SinglePane.SelectedContent.Title;
            }
            else if (floatingWindow.Model is LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow)
            {
                return app + ((LayoutAnchorablePane)layoutAnchorableFloatingWindow.SinglePane).SelectedContent.Title;
            }

            return Application.Current?.MainWindow.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}