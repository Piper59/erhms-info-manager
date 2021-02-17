﻿using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ERHMS.Desktop.Converters
{
    public class PhaseToColorConverter : IValueConverter
    {
        public byte Alpha { get; set; } = 0xff;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = ((Phase)value).ToColor();
            color.A = parameter == null ? Alpha : (byte)parameter;
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}