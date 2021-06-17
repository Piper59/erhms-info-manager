using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTable : XElement
    {
        public static XTable Create(XName name, DataTable table)
        {
            return new XTable(name)
            {
                TableName = table.TableName
            };
        }

        public string TableName
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public IEnumerable<XItem> XItems => Elements(ElementNames.Item).Cast<XItem>();

        public XTable(XName name)
            : base(name)
        {
            this.VerifyName(ElementNames.SourceTable, ElementNames.GridTable);
        }

        public XTable(XElement element)
            : this(element.Name)
        {
            Add(element.Attributes());
            Add(element.Elements(ElementNames.Item).Select(child => new XItem(child)));
        }

        public DataTable Instantiate()
        {
            return new DataTable(TableName);
        }
    }
}
