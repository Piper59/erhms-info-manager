using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERHMS.Common.IO
{
    public class CsvReader : IDisposable
    {
        private enum Mode
        {
            Initial,
            Unquoted,
            Quoted,
            Escaping
        }

        [Flags]
        private enum Flags
        {
            None = 0,
            EndOfValue = 1,
            EndOfRow = (1 << 1) | EndOfValue,
            EndOfFile = (1 << 2) | EndOfRow
        }

        private Mode mode;

        public TextReader Reader { get; }
        public int FieldCount { get; private set; } = -1;
        public int RowNumber { get; private set; }

        public CsvReader(TextReader reader)
        {
            Reader = reader;
        }

        protected Exception GetException(string reason, Exception innerException = null)
        {
            return new IOException($"{reason} in row {RowNumber}.", innerException);
        }

        private int ReadChar(out Flags flags)
        {
            flags = Flags.None;
            int ch = Reader.Read();
            switch (ch)
            {
                case -1:
                    switch (mode)
                    {
                        case Mode.Quoted:
                            throw GetException("Unexpected end of file");
                        default:
                            flags = Flags.EndOfFile;
                            return -1;
                    }
                case '\r':
                case '\n':
                    switch (mode)
                    {
                        case Mode.Quoted:
                            return ch;
                        default:
                            if (ch == '\r' && Reader.Peek() == '\n')
                            {
                                Reader.Read();
                            }
                            flags = Flags.EndOfRow;
                            return -1;
                    }
                case ',':
                    switch (mode)
                    {
                        case Mode.Quoted:
                            return ch;
                        default:
                            flags = Flags.EndOfValue;
                            return -1;
                    }
                case '"':
                    switch (mode)
                    {
                        case Mode.Initial:
                            mode = Mode.Quoted;
                            return -1;
                        case Mode.Unquoted:
                            throw GetException("Unexpected quote");
                        case Mode.Quoted:
                            mode = Mode.Escaping;
                            return -1;
                        case Mode.Escaping:
                            mode = Mode.Quoted;
                            return ch;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mode));
                    }
                default:
                    switch (mode)
                    {
                        case Mode.Initial:
                            mode = Mode.Unquoted;
                            goto default;
                        case Mode.Escaping:
                            throw GetException("Unexpected quote");
                        default:
                            return ch;
                    }
            }
        }

        private string ReadValue(out Flags flags)
        {
            mode = Mode.Initial;
            StringBuilder value = new StringBuilder();
            while (true)
            {
                int ch = ReadChar(out flags);
                if (ch != -1)
                {
                    value.Append((char)ch);
                }
                if (flags.HasFlag(Flags.EndOfValue))
                {
                    return value.ToString();
                }
            }
        }

        public IList<string> ReadRow()
        {
            string value = ReadValue(out Flags flags);
            if (value == "" && flags.HasFlag(Flags.EndOfFile))
            {
                return null;
            }
            RowNumber++;
            IList<string> values = new List<string>
            {
                value
            };
            while (!flags.HasFlag(Flags.EndOfRow))
            {
                values.Add(ReadValue(out flags));
            }
            if (FieldCount == -1)
            {
                FieldCount = values.Count;
            }
            else if (values.Count != FieldCount)
            {
                throw GetException($"Unexpected field count {values.Count} (expected {FieldCount})");
            }
            return values;
        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}
