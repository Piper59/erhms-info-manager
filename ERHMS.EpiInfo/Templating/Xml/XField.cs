using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Infrastructure;
using ERHMS.EpiInfo.Templating.Xml.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
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

        public static XField Create(DataRow field)
        {
            XField xField = new XField();
            foreach (DataColumn column in field.Table.Columns)
            {
                xField.SetAttributeValueEx(field[column], column.ColumnName);
            }
            xField.OnCreated();
            return xField;
        }

        public int FieldId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int PageId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int FieldTypeId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public MetaFieldType FieldType
        {
            get { return (MetaFieldType)FieldTypeId; }
            set { FieldTypeId = (int)value; }
        }

        public string List
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public string RelateCondition
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int? RelatedViewId
        {
            get { return (int?)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int? SourceFieldId
        {
            get { return (int?)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public double? TabIndex
        {
            get { return (double?)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public XPage XPage => (XPage)Parent;

        public XField()
            : base(ElementNames.Field) { }

        public XField(XElement element)
            : base(element)
        {
            OnCreated();
        }

        private void OnCreated()
        {
            XAttribute backgroundColor = Attribute(ColumnNames.BACKGROUND_COLOR);
            if (backgroundColor != null && backgroundColor.Value == "")
            {
                backgroundColor.Remove();
            }
            if (!ConfigurationExtensions.CompatibilityMode)
            {
                foreach (string columnName in IgnoredColumnNames)
                {
                    Attribute(columnName)?.Remove();
                }
            }
        }

        public Field Instantiate(Page page)
        {
            Field field = page.CreateField(FieldType);
            field.Name = Name;
            foreach (IFieldMapper mapper in Mappers)
            {
                mapper.SetProperties(this, field);
            }
            return field;
        }
    }
}
