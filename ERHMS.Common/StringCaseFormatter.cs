using System;
using System.Globalization;

namespace ERHMS.Common
{
    public class StringCaseFormatter : ICustomFormatter, IFormatProvider
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return "";
            }
            if (format != null && arg is string str)
            {
                switch (format.ToUpper())
                {
                    case "L":
                        return Culture.TextInfo.ToLower(str);
                    case "U":
                        return Culture.TextInfo.ToUpper(str);
                    case "T":
                        return Culture.TextInfo.ToTitleCase(str);
                }
            }
            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, Culture);
            }
            return arg.ToString();
        }
    }
}
