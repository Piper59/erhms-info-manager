using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTable : XElement
    {
        private const string Space = " ";
        private const string EscapedSpace = "__space__";

        private static string Escape(string columnName)
        {
            return columnName.Replace(Space, EscapedSpace);
        }

        private static string Unescape(string attributeName)
        {
            return attributeName.Replace(EscapedSpace, Space);
        }

        public static XTable Create(string elementName, DataTable table)
        {
            XTable xTable = new XTable(elementName)
            {
                TableName = table.TableName
            };
            foreach (DataRow item in table.Rows)
            {
                XElement xItem = new XElement(ElementNames.Item);
                foreach (DataColumn column in table.Columns)
                {
                    string attributeName = Escape(column.ColumnName);
                    object value = item[column];
                    xItem.SetAttributeValue(value, attributeName);
                }
                xTable.Add(xItem);
            }
            return xTable;
        }

        public string TableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public IEnumerable<XElement> XItems => Elements(ElementNames.Item);

        public XTable(string elementName)
            : base(elementName) { }

        public XTable(XElement element)
            : base(element) { }

        public DataTable Instantiate()
        {
            DataTable table = new DataTable(TableName);
            foreach (XElement xItem in XItems)
            {
                foreach (XAttribute attribute in xItem.Attributes())
                {
                    string columnName = Unescape(attribute.Name.LocalName);
                    if (!table.Columns.Contains(columnName))
                    {
                        table.Columns.Add(columnName);
                    }
                }
                DataRow item = table.NewRow();
                foreach (XAttribute attribute in xItem.Attributes())
                {
                    string columnName = Unescape(attribute.Name.LocalName);
                    item.SetField(columnName, attribute.Value);
                }
                table.Rows.Add(item);
            }
            return table;
        }
    }
}
