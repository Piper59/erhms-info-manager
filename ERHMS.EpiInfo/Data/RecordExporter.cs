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

        public RecordExporter(View view, TextWriter writer)
            : base(writer)
        {
            View = view;
        }

        private IEnumerable<Field> GetFields()
        {
            return View.Fields.DataFields.Cast<Field>();
        }

        private IEnumerable<string> GetHeaders()
        {
            return GetFields().Select(field => field.Name);
        }

        private IEnumerable<string> GetValues(Record record)
        {
            foreach (Field field in GetFields())
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
