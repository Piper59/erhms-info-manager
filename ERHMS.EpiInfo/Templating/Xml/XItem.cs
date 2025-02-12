﻿using ERHMS.Common.Linq;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XItem : XElement
    {
        private const string Space = " ";
        private const string EscapedSpace = "__space__";

        private static string GetAttributeName(DataColumn column)
        {
            return column.ColumnName.Replace(Space, EscapedSpace);
        }

        private static string GetColumnName(XAttribute attribute)
        {
            return attribute.Name.LocalName.Replace(EscapedSpace, Space);
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
            element.VerifyName(ElementNames.Item);
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
            foreach ((string columnName, string value) in valuesByColumnName)
            {
                item[columnName] = value;
            }
            return item;
        }
    }
}
