using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Infrastructure;
using ERHMS.EpiInfo.Templating.Xml.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XField : XElement
    {
        private static readonly IReadOnlyCollection<IFieldMapper> Mappers = new IFieldMapper[]
        {
            new DateFieldMapper(),
            new DDLFieldOfCodesMapper(),
            new FieldWithSeparatePromptMapper(),
            new GroupFieldMapper(),
            new ImageFieldMapper(),
            new InputFieldWithoutSeparatePromptMapper(),
            new InputFieldWithSeparatePromptMapper(),
            new MirrorFieldMapper(),
            new NumberFieldMapper(),
            new OptionFieldMapper(),
            new PhoneNumberFieldMapper(),
            new RelatedViewFieldMapper(),
            new RenderableFieldMapper(),
            new TableBasedDropDownFieldMapper(),
            new TextFieldMapper()
        };

        public static XField Create(DataRow field)
        {
            XField xField = new XField();
            foreach (DataColumn column in field.Table.Columns)
            {
                xField.SetAttributeValue(field[column], column.ColumnName);
            }
            return xField;
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int PageId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string PageName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int FieldId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public Guid? UniqueId
        {
            get { return this.GetAttributeValueOrNull<Guid>(); }
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

        public string RelateCondition
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int? RelatedViewId
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public string List
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int? BackgroundColor
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public double? TabIndex
        {
            get { return this.GetAttributeValueOrNull<double>(); }
            set { this.SetAttributeValue(value); }
        }

        public int? SourceFieldId
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public XPage XPage => (XPage)Parent;

        public XField()
            : base(ElementNames.Field) { }

        public XField(XElement element)
            : base(element) { }

        public bool TryGetFont(string propertyName, out Font font)
        {
            if (this.TryGetAttribute($"{propertyName}Family", out XAttribute xFamily)
                && this.TryGetAttribute($"{propertyName}Size", out XAttribute xSize)
                && this.TryGetAttribute($"{propertyName}Style", out XAttribute xStyle))
            {
                FontFamily family = new FontFamily((string)xFamily);
                float size = (float)xSize;
                FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), (string)xStyle);
                font = new Font(family, size, style);
                return true;
            }
            else
            {
                font = null;
                return false;
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
