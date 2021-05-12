using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ERHMS.Common.Text
{
    public class ByteCountFormatter : ICustomFormatter, IFormatProvider
    {
        private class Unit
        {
            public static Unit Kilobyte { get; } = new Unit(1L << 10, "KB");
            public static Unit Megabyte { get; } = new Unit(1L << 20, "MB");
            public static Unit Gigabyte { get; } = new Unit(1L << 30, "GB");

            public static IEnumerable<Unit> Instances { get; } = new Unit[]
            {
                Kilobyte,
                Megabyte,
                Gigabyte
            };

            public long Magnitude { get; }
            public string Symbol { get; }

            private Unit(long magnitude, string symbol)
            {
                Magnitude = magnitude;
                Symbol = symbol;
            }
        }

        private static readonly IEnumerable<Unit> unitsDescending =
            Unit.Instances.OrderByDescending(unit => unit.Magnitude).ToList();

        private static bool TryGetValue(object arg, out long value)
        {
            try
            {
                value = (long)arg;
                return true;
            }
            catch (InvalidCastException)
            {
                value = default;
                return false;
            }
        }

        public static string Format(string format, long value)
        {
            long absValue = Math.Abs(value);
            Unit unit = unitsDescending.FirstOrDefault(_unit => _unit.Magnitude <= absValue) ?? Unit.Kilobyte;
            double unitValue = (double)value / unit.Magnitude;
            return $"{unitValue.ToString(format)} {unit.Symbol}";
        }

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
            if (TryGetValue(arg, out long value))
            {
                return Format(format, value);
            }
            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
            return arg.ToString();
        }
    }
}
