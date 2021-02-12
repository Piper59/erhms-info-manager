using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTable : XElement
    {
        private const string Space = " ";
        private const string EscapedSpace = "__space__";

        private static string GetAttributeName(string columnName)
        {
            return columnName.Replace(Space, EscapedSpace);
        }

        private static string GetColumnName(string attributeName)
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
                    string attributeName = GetAttributeName(column.ColumnName);
                    xItem.SetOrClearAttributeValue(item[column], attributeName);
                }
                xTable.Add(xItem);
            }
            return xTable;
        }

        public string TableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
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
                    string columnName = GetColumnName(attribute.Name.LocalName);
                    if (!table.Columns.Contains(columnName))
                    {
                        table.Columns.Add(columnName);
                    }
                }
                DataRow item = table.NewRow();
                foreach (XAttribute attribute in xItem.Attributes())
                {
                    string columnName = GetColumnName(attribute.Name.LocalName);
                    item[columnName] = attribute.Value;
                }
                table.Rows.Add(item);
            }
            return table;
        }
    }
}
