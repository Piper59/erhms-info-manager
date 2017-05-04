using Epi;
using ERHMS.EpiInfo;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ViewToWebSurveyUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            View view = (View)value;
            return view.IsWebSurvey() ? view.GetWebSurveyUrl() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
