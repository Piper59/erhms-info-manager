using System.Data;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XTable : XElement
    {
        private const string Space = "__space__";

        public string TableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public XTable(string elementName, DataTable table)
            : base(elementName)
        {
            TableName = table.TableName;
            foreach (DataRow row in table.Rows)
            {
                XElement xRow = new XElement(ElementNames.Row);
                foreach (DataColumn column in table.Columns)
                {
                    string attributeName = column.ColumnName.Replace(" ", Space);
                    xRow.SetAttributeValue(row[column], attributeName);
                }
                Add(xRow);
            }
        }

        public XTable(XElement element)
            : base(element) { }
    }
}
