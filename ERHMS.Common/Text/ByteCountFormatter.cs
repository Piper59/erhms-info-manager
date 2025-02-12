﻿using System;
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

        private static bool TryGetValue(object arg, out long value)
        {
            try
            {
                value = (long)arg;
                return true;
            }
            catch (InvalidCastException)
            {
                value = default(long);
                return false;
            }
        }

        public static string Format(string format, long value)
        {
            Unit unit = Unit.Instances.OrderByDescending(_unit => _unit.Magnitude)
                .FirstOrDefault(_unit => _unit.Magnitude <= Math.Abs(value))
                ?? Unit.Kilobyte;
            long unitValue = Math.DivRem(value, unit.Magnitude, out long remainder);
            if (remainder != 0)
            {
                unitValue += Math.Sign(value);
            }
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
