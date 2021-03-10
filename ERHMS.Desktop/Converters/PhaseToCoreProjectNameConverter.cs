using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class PhaseToCoreProjectNameConverter : IValueConverter
    {
        private static readonly KeyToResXResourceConverter CoreProjectToNameConverter = new KeyToResXResourceConverter
        {
            Prefix = "CoreProject.Name."
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CoreProjectToNameConverter.Convert(((Phase)value).ToCoreProject(), targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
