using Epi;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.Metadata
{
    public class FieldDataRow
    {
        public static implicit operator DataRow(FieldDataRow field)
        {
            return field.Row;
        }

        public DataRow Row { get; }
        public int FieldId => Row.Field<int>(ColumnNames.FIELD_ID);
        public string Name => Row.Field<string>(ColumnNames.NAME);
        public double? TabIndex => Row.Field<double?>(ColumnNames.TAB_INDEX);
        public int FieldTypeId => Row.Field<int>(ColumnNames.FIELD_TYPE_ID);
        public MetaFieldType FieldType => (MetaFieldType)FieldTypeId;
        public string List => Row.Field<string>(ColumnNames.LIST);
        public IEnumerable<string> ListItems => List.Split(Constants.LIST_SEPARATOR);
        public short? Position => Row.Field<short?>(ColumnNames.POSITION);

        public FieldDataRow(DataRow row)
        {
            Row = row;
        }
    }
}
