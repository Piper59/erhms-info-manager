using System;
using System.Globalization;

namespace ERHMS.Common
{
    public class StringCaseFormatter : ICustomFormatter, IFormatProvider
    {
        private enum StringCase
        {
            Lower,
            Upper,
            Title
        }

        private static bool TryGetStringCase(string format, out StringCase value)
        {
            switch (format?.ToUpper())
            {
                case "L":
                    value = StringCase.Lower;
                    return true;
                case "U":
                    value = StringCase.Upper;
                    return true;
                case "T":
                    value = StringCase.Title;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        private TextInfo TextInfo => Culture.TextInfo;

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
            else if (arg.GetType() == typeof(string) && TryGetStringCase(format, out StringCase stringCase))
            {
                string str = (string)arg;
                switch (stringCase)
                {
                    case StringCase.Lower:
                        return TextInfo.ToLower(str);
                    case StringCase.Upper:
                        return TextInfo.ToUpper(str);
                    case StringCase.Title:
                        return TextInfo.ToTitleCase(str);
                    default:
                        throw new FormatException($"Format specifier '{format}' is not supported.");
                }
            }
            else if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, Culture);
            }
            else
            {
                return arg.ToString();
            }
        }
    }
}
