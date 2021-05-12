using System;
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
            if (name != ElementNames.SourceTable && name != ElementNames.GridTable)
            {
                throw new ArgumentException($"Unexpected element name '{name}'.");
            }
        }

        public XTable(XElement element)
            : this(element.Name)
        {
            Add(element.Elements(ElementNames.Item).Select(child => new XItem(child)));
        }

        public DataTable Instantiate()
        {
            return new DataTable(TableName);
        }
    }
}
