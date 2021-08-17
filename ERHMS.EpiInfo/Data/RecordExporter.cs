using Epi;
using Epi.Fields;
using ERHMS.Common.IO;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ERHMS.EpiInfo.Data
{
    public class RecordExporter : CsvWriter
    {
        private readonly IEnumerable<Field> fields;

        public View View { get; }
        public IProgress<int> Progress { get; set; }

        public RecordExporter(View view, TextWriter writer)
            : base(writer)
        {
            View = view;
            fields = View.Fields.DataFields.Cast<Field>()
                .OrderBy(field => field, new FieldComparer.ByTabIndex())
                .ToList();
        }

        private IEnumerable<string> GetHeaders()
        {
            return fields.Select(field => field.Name);
        }

        private IEnumerable<string> GetValues(Record record)
        {
            foreach (Field field in fields)
            {
                yield return record.GetProperty(field.Name)?.ToString() ?? "";
            }
        }

        public void Export(CancellationToken cancellationToken)
        {
            Log.Instance.Debug("Exporting records");
            WriteRow(GetHeaders());
            using (RecordRepository repository = new RecordRepository(View))
            {
                foreach (Record record in repository.Select())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Progress?.Report(RowNumber + 1);
                    WriteRow(GetValues(record));
                }
            }
        }

        public void Export()
        {
            Export(CancellationToken.None);
        }
    }
}
