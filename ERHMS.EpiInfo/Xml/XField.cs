using Epi;
using Epi.Fields;
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
        private static readonly ICollection<IFieldMapper> Mappers = new IFieldMapper[]
        {
            new RenderableFieldMapper(),
            new FieldWithSeparatePromptMapper(),
            new InputFieldWithoutSeparatePromptMapper(),
            new InputFieldWithSeparatePromptMapper(),
            new TextFieldMapper(),
            new NumberFieldMapper(),
            new PhoneNumberFieldMapper(),
            new DateFieldMapper(),
            new OptionFieldMapper(),
            new ImageFieldMapper(),
            new MirrorFieldMapper(),
            new TableBasedDropDownFieldMapper(),
            new DDLFieldOfCodesMapper(),
            new RelatedViewFieldMapper(),
            new GroupFieldMapper()
        };

        public static XField Construct(DataRow field)
        {
            XField xField = new XField();
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

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int FieldId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int FieldTypeId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public MetaFieldType FieldType
        {
            get { return (MetaFieldType)FieldTypeId; }
            set { FieldTypeId = (int)value; }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public XPage XPage => (XPage)Parent;

        private XField()
            : base(ElementNames.Field) { }

        private XField(XElement element)
            : base(element) { }

        public Field Instantiate(Page page)
        {
            Field field = page.CreateField(FieldType);
            field.Name = Name;
            foreach (IFieldMapper mapper in Mappers)
            {
                mapper.Map(this, field);
            }
            return field;
        }
    }
}
