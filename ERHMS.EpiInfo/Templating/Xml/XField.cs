using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                if (column.ColumnName == nameof(BackgroundColor) && field.IsNull(column))
                {
                    continue;
                }
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

        public int FieldId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public Guid? UniqueId
        {
            get { return this.GetAttributeValue<Guid>(); }
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

        public string PageName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string RelateCondition
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int? RelatedViewId
        {
            get { return this.GetAttributeValue<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public string List
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public IEnumerable<string> ListItems
        {
            get { return List.Split(Constants.LIST_SEPARATOR); }
            set { List = string.Join(Constants.LIST_SEPARATOR.ToString(), value); }
        }

        public string SourceTableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int? BackgroundColor
        {
            get { return this.GetAttributeValue<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public double? TabIndex
        {
            get { return this.GetAttributeValue<double>(); }
            set { this.SetAttributeValue(value); }
        }

        public int? SourceFieldId
        {
            get { return this.GetAttributeValue<int>(); }
            set { this.SetAttributeValue(value); }
        }

        public XPage XPage => (XPage)Parent;

        public XField()
            : base(ElementNames.Field) { }

        public XField(XElement element)
            : this()
        {
            if (element.Name != ElementNames.Field)
            {
                throw new ArgumentException($"Unexpected element name '{element.Name}'.");
            }
            Add(element.Attributes());
        }

        public Field Instantiate(Page page)
        {
            Field field = page.CreateField(FieldType);
            field.Name = Name;
            return field;
        }

        private IEnumerable<XAttribute> GetCanonizedAttributes()
        {
            bool usedNameMap = false;
            IReadOnlyDictionary<string, string> nameMap = new Dictionary<string, string>
            {
                { "Expr1015", "ControlFontFamily" },
                { "Expr1016", "ControlFontSize" },
                { "Expr1017", "ControlFontStyle" }
            };
            IEnumerable<string> duplicateNames = new HashSet<string>
            {
                "ControlFontFamily1",
                "ControlFontSize1",
                "ControlFontStyle1"
            };
            foreach (XAttribute attribute in Attributes())
            {
                string name = attribute.Name.LocalName;
                string value = attribute.Value;
                if (usedNameMap && nameMap.Values.Contains(name))
                {
                    continue;
                }
                if (duplicateNames.Contains(name))
                {
                    continue;
                }
                if (nameMap.TryGetValue(name, out name))
                {
                    usedNameMap = true;
                }
                if (name.EndsWith("Percentage"))
                {
                    if (double.TryParse(value, out double result))
                    {
                        value = result.ToString("F6");
                    }
                }
                yield return new XAttribute(name, value);
            }
        }

        // TODO: Address differences between Access and SQL Server
        public void Canonize()
        {
            UniqueId = null;
            ReplaceAttributes(GetCanonizedAttributes());
        }
    }
}
