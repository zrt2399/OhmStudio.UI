﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OhmStudio.UI.Converters
{
    public class DropShadowMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom);
            }
            return default(Thickness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}