using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XTable : XElement
    {
        private const string Space = "__space__";

        public static XTable Construct(string elementName, DataTable table)
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
                    xRow.SetAttributeValue(row[column], attributeName);
                }
                xTable.Add(xRow);
            }
            return xTable;
        }

        public static XTable Wrap(XElement element)
        {
            return new XTable(element);
        }

        public string TableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        private XTable(string elementName)
            : base(elementName) { }

        private XTable(XElement element)
            : base(element) { }

        public DataTable Instantiate()
        {
            DataTable table = new DataTable(TableName);
            foreach (XElement xRow in Elements(ElementNames.Row))
            {
                ICollection<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>();
                foreach (XAttribute attribute in Attributes())
                {
                    fields.Add(new KeyValuePair<string, string>(
                        attribute.Name.ToString().Replace(Space, " "),
                        attribute.Value));
                }
                foreach (KeyValuePair<string, string> field in fields)
                {
                    if (!table.Columns.Contains(field.Key))
                    {
                        table.Columns.Add(field.Key);
                    }
                }
                DataRow row = table.NewRow();
                foreach (KeyValuePair<string, string> field in fields)
                {
                    row.SetField(field.Key, field.Value);
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
