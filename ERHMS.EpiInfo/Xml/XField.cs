using Epi;
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

        public static XField Construct(DataRow field)
        {
            XField xField = new XField();
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
                xField.SetAttributeValue(field[column], column.ColumnName);
            }
            return xField;
        }

        public static XField Wrap(XElement element)
        {
            return new XField(element);
        }

        public new string Name => (string)this.GetAttribute();
        public int FieldId => (int)this.GetAttribute();
        public int FieldTypeId => (int)this.GetAttribute();
        public MetaFieldType FieldType => (MetaFieldType)FieldTypeId;
        public string SourceTableName => (string)this.GetAttribute();

        private XField()
            : base(ElementNames.Field) { }

        private XField(XElement element)
            : base(element) { }
    }
}
