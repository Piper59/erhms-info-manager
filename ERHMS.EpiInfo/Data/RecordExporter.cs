using Epi;
using Epi.Fields;
using ERHMS.Data.Transport;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class RecordExporter<TRecord> : Exporter<TRecord>
        where TRecord : Record
    {
        public View View { get; }

        public RecordExporter(TextWriter writer, View view)
            : base(writer)
        {
            View = view;
        }

        private IEnumerable<Field> GetFields()
        {
            return View.Fields.TableColumnFields.Cast<Field>();
        }

        protected override IEnumerable<string> GetHeaders()
        {
            return GetFields().Select(field => field.Name);
        }

        protected override IEnumerable<string> GetFields(TRecord entity)
        {
            foreach (Field field in GetFields())
            {
                yield return entity.GetProperty(field.Name)?.ToString() ?? "";
            }
        }
    }

    public class RecordExporter : RecordExporter<Record>
    {
        public RecordExporter(TextWriter writer, View view)
            : base(writer, view) { }
    }
}
