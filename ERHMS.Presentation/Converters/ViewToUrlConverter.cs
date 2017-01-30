using Epi;
using ERHMS.EpiInfo;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ViewToUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            View view = (View)value;
            if (view == null)
            {
                return null;
            }
            else
            {
                return view.IsWebSurvey() ? view.GetWebSurveyUrl().ToString() : "Unpublished";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
