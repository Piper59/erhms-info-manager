using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections;
using Microsoft.Maps;
using Microsoft.Maps.MapControl.WPF;

namespace ERHMS.Presentation.Converters
{
    class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return new Location(39.828328, -98.579416);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return "";
        }
    }
}
