using System;
using System.Globalization;

namespace ERHMS.Common
{
    public class StringCaseFormatter : ICustomFormatter, IFormatProvider
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        private TextInfo TextInfo => Culture.TextInfo;

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string str = arg?.ToString();
            switch (format?.ToUpper())
            {
                case "U":
                    return TextInfo.ToUpper(str);
                case "L":
                    return TextInfo.ToLower(str);
                case "T":
                    return TextInfo.ToTitleCase(str);
                default:
                    return arg is IFormattable formattable ? formattable.ToString(format, formatProvider) : str;
            }
        }
    }
}
