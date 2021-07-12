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
        private class FieldComparer : IComparer<Field>
        {
            public int Compare(Field field1, Field field2)
            {
                int result = 0;
                if (field1 is RenderableField renderableField1 && field2 is RenderableField renderableField2)
                {
                    result = Comparer<int>.Default.Compare(
                        renderableField1.Page.Position,
                        renderableField2.Page.Position);
                    if (result == 0)
                    {
                        result = Comparer<double>.Default.Compare(renderableField1.TabIndex, renderableField2.TabIndex);
                    }
                }
                if (result == 0)
                {
                    result = Comparer<int>.Default.Compare(field1.Id, field2.Id);
                }
                return result;
            }
        }

        private IEnumerable<Field> fields;

        public View View { get; }
        public int RecordCount { get; private set; }

        public RecordExporter(View view, TextWriter writer)
            : base(writer)
        {
            View = view;
            fields = View.Fields.DataFields.Cast<Field>()
                .OrderBy(field => field, new FieldComparer())
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

        public void Export()
        {
            WriteRow(GetHeaders().ToList());
            using (RecordRepository repository = new RecordRepository(View))
            {
                foreach (Record record in repository.Select())
                {
                    WriteRow(GetValues(record).ToList());
                    RecordCount++;
                }
            }
        }
    }
}
