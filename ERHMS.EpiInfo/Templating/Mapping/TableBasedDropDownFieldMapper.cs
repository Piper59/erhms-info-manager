﻿using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class TableBasedDropDownFieldMapper : FieldMapper<TableBasedDropDownField>
    {
        protected override MetaFieldType? FieldType => null;
        protected override FieldPropertySetterCollection<TableBasedDropDownField> Setters { get; } =
            new FieldPropertySetterCollection<TableBasedDropDownField>
            {
                { field => field.ShouldSort, ColumnNames.SORT },
                { field => field.TextColumnName },
                { field => field.CodeColumnName },
                { field => field.SourceTableName },
                { field => field.IsExclusiveTable }
            };

        public override bool MapProperties(TableBasedDropDownField field)
        {
            bool changed = false;
            if (Context.MapTableName(field.SourceTableName, out string result))
            {
                field.SourceTableName = result;
                changed = true;
            }
            return changed;
        }

        public override bool MapAttributes(XField xField)
        {
            bool changed = false;
            if (Context.MapTableName(xField.SourceTableName, out string result))
            {
                xField.SourceTableName = result;
                changed = true;
            }
            return changed;
        }
    }
}
