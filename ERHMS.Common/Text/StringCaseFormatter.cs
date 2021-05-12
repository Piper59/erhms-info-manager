using System;
using System.Globalization;

namespace ERHMS.Common.Text
{
    public class StringCaseFormatter : ICustomFormatter, IFormatProvider
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, string value)
        {
            switch (format.ToUpper())
            {
                case "L":
                    return Culture.TextInfo.ToLower(value);
                case "U":
                    return Culture.TextInfo.ToUpper(value);
                case "T":
                    return Culture.TextInfo.ToTitleCase(value);
            }
            throw new FormatException($"Format specifier '{format}' is not supported.");
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return "";
            }
            if (format != null && arg is string value)
            {
                try
                {
                    return Format(format, value);
                }
                catch (FormatException) { }
            }
            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, Culture);
            }
            return arg.ToString();
        }
    }
}
