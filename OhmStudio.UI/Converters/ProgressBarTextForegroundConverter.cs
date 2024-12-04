using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OhmStudio.UI.Converters
{
    public class ProgressBarTextForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var actualWidth = (double)values[0];
            var textWidth = (double)values[2];
            var min = (double)values[4];
            var max = (double)values[5];
            var value = (double)values[6];
            //var orientation = (Orientation)values[7];
            var foreground = values[8] as Brush;
            var invertedForeground = Brushes.White /*values[9] as Brush*/;

            if (textWidth == 0 || double.IsNaN(textWidth) || actualWidth == 0 || double.IsNaN(actualWidth))
            {
                return foreground;
            }

            var totalPercent = ((value - min) / (max - min));
            var percentWidth = actualWidth * totalPercent;
            var innerWidth = percentWidth - ((actualWidth - textWidth) / 2);
            if (innerWidth <= 0 || double.IsNaN(innerWidth))
            {
                return foreground;
            }
            else if (innerWidth >= textWidth)
            {
                return invertedForeground;
            }

            var innerPercent = innerWidth / textWidth;
            return GetStackedVisualBrush(invertedForeground, foreground, Orientation.Horizontal, innerPercent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static VisualBrush GetStackedVisualBrush(Brush brush1, Brush brush2, Orientation oritention, double offset)
        {
            var grid = new Grid()
            {
                Width = 1,
                Height = 1,
            };
            if (oritention == Orientation.Horizontal)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(offset, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1 - offset, GridUnitType.Star) });

                var rectangle1 = new Rectangle() { Fill = brush1 };
                grid.Children.Add(rectangle1);

                var rectangle2 = new Rectangle() { Fill = brush2 };
                Grid.SetColumn(rectangle2, 1);
                grid.Children.Add(rectangle2);
            }
            else
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(offset, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1 - offset, GridUnitType.Star) });

                var rectangle1 = new Rectangle() { Fill = brush1 };
                grid.Children.Add(rectangle1);

                var rectangle2 = new Rectangle() { Fill = brush2 };
                Grid.SetRow(rectangle2, 1);
                grid.Children.Add(rectangle2);
            }

            return new VisualBrush(grid);
        }
    }
}