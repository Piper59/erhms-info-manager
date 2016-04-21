using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ERHMS.WPF.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                if((bool)value)
                {
                    return "Hidden";
                }
                else
                {
                    return "Visible";
                }
            }
            catch(Exception)
            {
                return "Hidden";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
