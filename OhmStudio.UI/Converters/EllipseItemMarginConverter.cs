using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Converters
{
    public class EllipseItemMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double hight = (double)values[1];
            Dock orientation = (Dock)values[2];
            Thickness borderThickness = (Thickness)values[3];
            ShapeType shapeType = (ShapeType)values[4];
            double shear = (double)values[5];

            double halfWidth = width / 2;
            double halfHight = hight / 2;
            double thickness = new double[] { borderThickness.Left, borderThickness.Top, borderThickness.Right, borderThickness.Bottom }.Max();
            double halfThickness = thickness / 2;

            if (orientation is Dock.Left or Dock.Right)
            {
                return shapeType == ShapeType.Parallelogram
                    ? new Thickness(GetOffset(shear, hight) - thickness - halfWidth)
                    : new Thickness(-(halfWidth + halfThickness));
            }
            else if (orientation is Dock.Top or Dock.Bottom)
            {
                return new Thickness(-(halfHight + halfThickness));
            }

            return new Thickness();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double GetOffset(double shear, double hight)
        {
            double cx = (shear + 0) / 2;
            double cy = (0 + hight) / 2;
            double distance = Math.Sqrt(cx * cx + cy * cy);
            return distance;
        }
    }
}