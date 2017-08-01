using ERHMS.Domain;
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
            string webSurveyId = ((View)value).WebSurveyId;
            return ViewExtensions.IsWebSurvey(webSurveyId) ? ViewExtensions.GetWebSurveyUrl(webSurveyId) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
