using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XItem : XElement
    {
        private static readonly string space = " ";
        private static readonly string escapedSpace = "__space__";

        private static string GetAttributeName(DataColumn column)
        {
            return column.ColumnName.Replace(space, escapedSpace);
        }

        private static string GetColumnName(XAttribute attribute)
        {
            return attribute.Name.LocalName.Replace(escapedSpace, space);
        }

        public static XItem Create(DataRow item)
        {
            XItem xItem = new XItem();
            foreach (DataColumn column in item.Table.Columns)
            {
                string attributeName = GetAttributeName(column);
                xItem.SetAttributeValue(item[column], attributeName);
            }
            return xItem;
        }

        public XTable XTable => (XTable)Parent;

        public XItem()
            : base(ElementNames.Item) { }

        public XItem(XElement element)
            : this()
        {
            if (element.Name != ElementNames.Item)
            {
                throw new ArgumentException($"Unexpected element name '{element.Name}'.");
            }
            Add(element.Attributes());
        }

        public DataRow Instantiate(DataTable table)
        {
            IDictionary<string, string> valuesByColumnName = new Dictionary<string, string>();
            foreach (XAttribute attribute in Attributes())
            {
                string columnName = GetColumnName(attribute);
                if (!table.Columns.Contains(columnName))
                {
                    table.Columns.Add(columnName);
                }
                valuesByColumnName[columnName] = attribute.Value;
            }
            DataRow item = table.NewRow();
            foreach (KeyValuePair<string, string> valueByColumnName in valuesByColumnName)
            {
                item[valueByColumnName.Key] = valueByColumnName.Value;
            }
            return item;
        }
    }
}
