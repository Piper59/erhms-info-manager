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
        public View View { get; }
        private IEnumerable<Field> Fields => View.Fields.DataFields.Cast<Field>();

        public RecordExporter(View view, TextWriter writer)
            : base(writer)
        {
            View = view;
        }

        private IEnumerable<string> GetHeaders()
        {
            return Fields.Select(field => field.Name);
        }

        private IEnumerable<string> GetValues(Record record)
        {
            foreach (Field field in Fields)
            {
                yield return record.GetProperty(field.Name)?.ToString() ?? "";
            }
        }

        public void Export()
        {
            WriteRow(GetHeaders().ToList());
            using (RecordRepository repository = new RecordRepository(View))
            {
                foreach (Record record in repository.Select())
                {
                    WriteRow(GetValues(record).ToList());
                }
            }
        }
    }
}
