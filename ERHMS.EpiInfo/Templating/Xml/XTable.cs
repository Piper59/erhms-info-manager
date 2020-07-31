using ERHMS.EpiInfo.Infrastructure;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTable : XElement
    {
        private const string Space = " ";
        private const string EscapedSpace = "__space__";

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
                    string attributeName = column.ColumnName.Replace(Space, EscapedSpace);
                    xItem.SetAttributeValue(item[column], attributeName);
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
                ICollection<(string, string)> fields = new List<(string, string)>();
                foreach (XAttribute attribute in xItem.Attributes())
                {
                    string columnName = attribute.Name.ToString().Replace(EscapedSpace, Space);
                    if (!table.Columns.Contains(columnName))
                    {
                        table.Columns.Add(columnName);
                    }
                    fields.Add((columnName, attribute.Value));
                }
                DataRow item = table.NewRow();
                foreach ((string columnName, string value) in fields)
                {
                    item.SetField(columnName, value);
                }
                table.Rows.Add(item);
            }
            return table;
        }
    }
}
