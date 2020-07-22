using ERHMS.EpiInfo.Infrastructure;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTable : XElement
    {
        private const string Space = "__space__";

        public static XTable Create(string elementName, DataTable table)
        {
            XTable xTable = new XTable(elementName)
            {
                TableName = table.TableName
            };
            foreach (DataRow row in table.Rows)
            {
                XElement xRow = new XElement(ElementNames.Row);
                foreach (DataColumn column in table.Columns)
                {
                    string attributeName = column.ColumnName.Replace(" ", Space);
                    xRow.SetAttributeValueEx(row[column], attributeName);
                }
                xTable.Add(xRow);
            }
            return xTable;
        }

        public string TableName
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public XTable(string elementName)
            : base(elementName) { }

        public XTable(XElement element)
            : base(element) { }

        public DataTable Instantiate()
        {
            DataTable table = new DataTable(TableName);
            foreach (XElement xRow in Elements(ElementNames.Row))
            {
                ICollection<(string, string)> fields = new List<(string, string)>();
                foreach (XAttribute attribute in xRow.Attributes())
                {
                    string columnName = attribute.Name.ToString().Replace(Space, " ");
                    string value = attribute.Value;
                    if (!table.Columns.Contains(columnName))
                    {
                        table.Columns.Add(columnName);
                    }
                    fields.Add((columnName, value));
                }
                DataRow row = table.NewRow();
                foreach ((string columnName, string value) in fields)
                {
                    row.SetField(columnName, value);
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
