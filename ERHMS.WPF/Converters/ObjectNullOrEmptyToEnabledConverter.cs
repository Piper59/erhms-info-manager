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

namespace ERHMS.WPF.Converters
{
    class ObjectNullOrEmptyToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return false;
            }
            else if (value is ICollection && ((ICollection)value).Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return "";
        }
    }
}
