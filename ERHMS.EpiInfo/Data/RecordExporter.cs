using Epi;
using Epi.Fields;
using ERHMS.Common.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class RecordExporter : CsvWriter
    {
        private static IEnumerable<string> GetHeaderValues(IEnumerable<Field> fields)
        {
            return fields.Select(field => field.Name);
        }

        private static IEnumerable<string> GetRecordValues(IEnumerable<Field> fields, Record record)
        {
            foreach (Field field in fields)
            {
                yield return record.GetProperty(field.Name)?.ToString() ?? "";
            }
        }

        public View View { get; }

        public RecordExporter(TextWriter writer, View view)
            : base(writer)
        {
            View = view;
        }

        public void Export()
        {
            IEnumerable<Field> fields = View.Fields.TableColumnFields.Cast<Field>().ToList();
            WriteValues(GetHeaderValues(fields));
            RecordRepository repository = new RecordRepository(View);
            foreach (Record record in repository.Select())
            {
                WriteValues(GetRecordValues(fields, record));
            }
        }
    }
}
