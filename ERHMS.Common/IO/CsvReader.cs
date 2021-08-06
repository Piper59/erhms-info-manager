using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERHMS.Common.IO
{
    public class CsvReader : IDisposable
    {
        private enum State
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

        private State state;
        private Flags flags;

        public TextReader Reader { get; }
        public int FieldCount { get; private set; } = -1;
        public int RowNumber { get; private set; }

        public CsvReader(TextReader reader)
        {
            Reader = reader;
        }

        protected Exception GetException(string reason, int rowNumber, Exception innerException = null)
        {
            return new IOException($"{reason} in row {rowNumber}.", innerException);
        }

        protected Exception GetException(string reason, Exception innerException = null)
        {
            return GetException(reason, RowNumber, innerException);
        }

        private int ReadChar()
        {
            flags = Flags.None;
            int ch = Reader.Read();
            switch (ch)
            {
                case -1:
                    switch (state)
                    {
                        case State.Quoted:
                            throw GetException("Unexpected end of file");
                        default:
                            flags = Flags.EndOfFile;
                            return -1;
                    }
                case '\r':
                case '\n':
                    switch (state)
                    {
                        case State.Quoted:
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
                    switch (state)
                    {
                        case State.Quoted:
                            return ch;
                        default:
                            flags = Flags.EndOfValue;
                            return -1;
                    }
                case '"':
                    switch (state)
                    {
                        case State.Initial:
                            state = State.Quoted;
                            return -1;
                        case State.Unquoted:
                            throw GetException("Unexpected quote");
                        case State.Quoted:
                            state = State.Escaping;
                            return -1;
                        case State.Escaping:
                            state = State.Quoted;
                            return ch;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state));
                    }
                default:
                    switch (state)
                    {
                        case State.Initial:
                            state = State.Unquoted;
                            goto default;
                        case State.Escaping:
                            throw GetException("Unexpected quote");
                        default:
                            return ch;
                    }
            }
        }

        private string ReadValue()
        {
            state = State.Initial;
            StringBuilder value = new StringBuilder();
            while (true)
            {
                int ch = ReadChar();
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
            string value = ReadValue();
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
                values.Add(ReadValue());
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
