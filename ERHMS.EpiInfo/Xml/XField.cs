using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XField : XElement
    {
        private static readonly ISet<string> IgnoredColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UniqueId",
            "Expr1015",
            "Expr1016",
            "Expr1017"
        };

        public new string Name => (string)this.GetAttribute();
        public int FieldId => (int)this.GetAttribute();
        public int FieldTypeId => (int)this.GetAttribute();
        public MetaFieldType FieldType => (MetaFieldType)FieldTypeId;
        public string SourceTableName => (string)this.GetAttribute();

        private XField()
            : base(ElementNames.Field) { }

        public XField(DataRow field)
            : this()
        {
            string name = field.Field<string>(ColumnNames.NAME);
            Log.Default.Debug($"Adding field: {name}");
            MetaFieldType fieldType = (MetaFieldType)field.Field<int>(ColumnNames.FIELD_TYPE_ID);
            foreach (DataColumn column in field.Table.Columns)
            {
                if (!ConfigurationExtensions.CompatibilityMode && IgnoredColumnNames.Contains(column.ColumnName))
                {
                    continue;
                }
                if (column.ColumnName == ColumnNames.BACKGROUND_COLOR && field.IsNull(column))
                {
                    continue;
                }
                this.SetAttributeValue(field[column], column.ColumnName);
            }
        }

        public XField(XElement element)
            : base(element) { }
    }
}
