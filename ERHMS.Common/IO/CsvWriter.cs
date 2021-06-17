using System;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.Common.IO
{
    public class CsvWriter
    {
        private static string Escape(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        private static string Quote(string value)
        {
            return $"\"{Escape(value)}\"";
        }

        public TextWriter Writer { get; }
        public int FieldCount { get; private set; } = -1;

        public CsvWriter(TextWriter writer)
        {
            Writer = writer;
        }

        private void WriteValue(string value)
        {
            foreach (char ch in value)
            {
                if (ch == '\r' || ch == '\n' || ch == ',' || ch == '"')
                {
                    Writer.Write(Quote(value));
                    return;
                }
            }
            Writer.Write(value);
        }

        public void WriteRow(IList<string> values)
        {
            if (FieldCount == -1)
            {
                if (values.Count == 0)
                {
                    throw new ArgumentException("Field count must be greater than zero.", nameof(values));
                }
                FieldCount = values.Count;
            }
            else if (values.Count != FieldCount)
            {
                throw new ArgumentException(
                    $"Unexpected field count {values.Count} (expected {FieldCount}).",
                    nameof(values));
            }
            WriteValue(values[0]);
            for (int index = 1; index < values.Count; index++)
            {
                Writer.Write(",");
                WriteValue(values[index]);
            }
            Writer.Write("\r\n");
        }
    }
}
