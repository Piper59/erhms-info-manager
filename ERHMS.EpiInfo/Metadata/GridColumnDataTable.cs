using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Metadata
{
    public class GridColumnDataTable : IEnumerable<GridColumnDataRow>
    {
        public static implicit operator DataTable(GridColumnDataTable fields)
        {
            return fields.Table;
        }

        public DataTable Table { get; }

        public GridColumnDataTable(DataTable table)
        {
            Table = table;
        }

        public IEnumerator<GridColumnDataRow> GetEnumerator()
        {
            return Table.AsEnumerable()
                .Select(row => new GridColumnDataRow(row))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
