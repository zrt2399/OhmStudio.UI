using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Converters
{
    public class StepItemEllipseItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                EllipseOrientation ellipseOrientation = (EllipseOrientation)values[0];
                StepType stepType = (StepType)values[1];
                if (stepType == StepType.Begin)
                {
                    return GetVisibility(ellipseOrientation is EllipseOrientation.Bottom);
                } 
                else if (stepType == StepType.End)
                {
                    return GetVisibility(ellipseOrientation is EllipseOrientation.Top or EllipseOrientation.Left);
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            return Binding.DoNothing;
        }

        private static Visibility GetVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}