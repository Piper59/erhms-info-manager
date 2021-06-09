using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ERHMS.Data.Transport
{
    public abstract class Exporter<TEntity>
    {
        private static readonly Regex controlStringRegex = new Regex(@"\r\n|\r|\n|""|,");

        private static string Escape(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        private static string Quote(string value)
        {
            return $"\"{Escape(value)}\"";
        }

        public TextWriter Writer { get; }

        protected Exporter(TextWriter writer)
        {
            Writer = writer;
        }

        private void Write(string value)
        {
            Writer.Write(controlStringRegex.IsMatch(value) ? Quote(value) : value);
        }

        private void Write(IEnumerable<string> values)
        {
            using (IEnumerator<string> enumerator = values.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    Write(enumerator.Current);
                }
                while (enumerator.MoveNext())
                {
                    Writer.Write(",");
                    Write(enumerator.Current);
                }
            }
            Writer.Write("\r\n");
        }

        protected abstract IEnumerable<string> GetHeaders();
        protected abstract IEnumerable<string> GetFields(TEntity entity);

        public void Export(IEnumerable<TEntity> entities)
        {
            Write(GetHeaders());
            foreach (TEntity entity in entities)
            {
                Write(GetFields(entity));
            }
        }
    }
}
