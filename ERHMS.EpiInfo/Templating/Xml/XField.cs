using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XField : XElement
    {
        public static XField Create(DataRow field)
        {
            XField xField = new XField();
            foreach (DataColumn column in field.Table.Columns)
            {
                xField.SetOrClearAttributeValue(field[column], column.ColumnName);
            }
            return xField;
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int PageId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int FieldId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public Guid? UniqueId
        {
            get { return this.GetAttributeValueOrNull<Guid>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int FieldTypeId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public MetaFieldType FieldType
        {
            get { return (MetaFieldType)FieldTypeId; }
            set { FieldTypeId = (int)value; }
        }

        public string RelateCondition
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int? RelatedViewId
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string List
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public IEnumerable<string> ListItems
        {
            get { return List.Split(Constants.LIST_SEPARATOR); }
            set { List = string.Join(Constants.LIST_SEPARATOR.ToString(), value); }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int? BackgroundColor
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public double? TabIndex
        {
            get { return this.GetAttributeValueOrNull<double>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int? SourceFieldId
        {
            get { return this.GetAttributeValueOrNull<int>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public XPage XPage => (XPage)Parent;

        public XField()
            : base(ElementNames.Field) { }

        public XField(XElement element)
            : base(element) { }

        public Field Instantiate(Page page)
        {
            Field field = page.CreateField(FieldType);
            field.Name = Name;
            return field;
        }
    }
}
