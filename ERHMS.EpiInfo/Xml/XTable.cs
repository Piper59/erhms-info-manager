using System;
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
    }
}
