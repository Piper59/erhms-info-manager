using System;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.Common.IO
{
    public class CsvWriter
    {
        private static readonly char[] controlChars = new char[]
        {
            '\r',
            '\n',
            ',',
            '"'
        };

        private static string Escape(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        private static string Quote(string value)
        {
            return $"\"{Escape(value)}\"";
        }

        public TextWriter Writer { get; }

        public CsvWriter(TextWriter writer)
        {
            Writer = writer;
        }

        private void WriteValue(string value)
        {
            Writer.Write(value.IndexOfAny(controlChars) == -1 ? value : Quote(value));
        }

        public void WriteValues(IEnumerable<string> values)
        {
            using (IEnumerator<string> enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new ArgumentException("Sequence is empty.", nameof(values));
                }
                WriteValue(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    Writer.Write(",");
                    WriteValue(enumerator.Current);
                }
            }
            Writer.Write("\r\n");
        }
    }
}
