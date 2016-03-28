using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Data;
using System.Globalization;

namespace ERHMS.WPF.Converters
{
    public class DateToShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                DateTime? date = (DateTime?)value;
                return date.Value.ToShortDateString();
            }
            catch(Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                DateTime date = DateTime.Parse(value.ToString());
                return date;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
