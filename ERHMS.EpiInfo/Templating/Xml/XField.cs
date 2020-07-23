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

        public int? BackgroundColor
        {
            get
            {
                if (int.TryParse((string)this.GetAttributeEx(), out int value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value);
            }
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
            get
            {
                if (int.TryParse((string)this.GetAttributeEx(), out int value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value);
            }
        }

        public int? SourceFieldId
        {
            get
            {
                if (int.TryParse((string)this.GetAttributeEx(), out int value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value);
            }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public double? TabIndex
        {
            get
            {
                if (double.TryParse((string)this.GetAttributeEx(), out double value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value);
            }
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
            if (BackgroundColor == null)
            {
                Attribute(nameof(BackgroundColor))?.Remove();
            }
            foreach (string columnName in IgnoredColumnNames)
            {
                Attribute(columnName)?.Remove();
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
