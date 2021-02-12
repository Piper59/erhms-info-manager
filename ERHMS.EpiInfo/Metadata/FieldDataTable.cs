using Epi;
using ERHMS.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.Metadata
{
    public class FieldDataTable : IEnumerable<FieldDataRow>
    {
        public static implicit operator DataTable(FieldDataTable fields)
        {
            return fields.Table;
        }

        public DataTable Table { get; }

        public FieldDataTable(DataTable table)
        {
            table.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            Table = table;
        }

        public IEnumerator<FieldDataRow> GetEnumerator()
        {
            return Table.AsEnumerable()
                .Select(row => new FieldDataRow(row))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
