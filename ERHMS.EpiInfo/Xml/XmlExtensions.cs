using Epi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    internal static class XmlExtensions
    {
        private const string OptionsSeparator = "||";
        private const string Space = "__space__";

        public static Color ToColor(this XAttribute @this)
        {
            try
            {
                return Color.FromArgb((int)@this);
            }
            catch
            {
                return Color.Empty;
            }
        }

        public static XElement ToElement(this DataTable @this, string elementName)
        {
            XElement xTable = new XElement(elementName,
                new XAttribute("TableName", @this.TableName)
            );
            foreach (DataRow row in @this.Rows)
            {
                XElement xRow = new XElement("Item");
                foreach (DataColumn column in @this.Columns)
                {
                    string attributeName = column.ColumnName.Replace(" ", Space);
                    xRow.Add(new XAttribute(attributeName, row[column]));
                }
                xTable.Add(xRow);
            }
            return xTable;
        }

        public static Font ToFont(this XElement @this, string prefix)
        {
            try
            {
                string family = (string)@this.Attribute($"{prefix}FontFamily");
                float size = (float)@this.Attribute($"{prefix}FontSize");
                if (!Enum.TryParse((string)@this.Attribute($"{prefix}FontStyle"), true, out FontStyle style))
                {
                    style = FontStyle.Regular;
                }
                return new Font(family, size, style);
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<string> ToOptions(this XAttribute @this)
        {
            try
            {
                string value = (string)@this;
                int index = value.IndexOf(OptionsSeparator);
                string options = index == -1 ? value : value.Substring(0, index);
                return options.Split(Constants.LIST_SEPARATOR);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public static DataTable ToTable(this XElement @this)
        {
            DataTable table = new DataTable
            {
                TableName = (string)@this.Attribute("TableName")
            };
            foreach (XElement xItem in @this.Elements("Item"))
            {
                ICollection<KeyValuePair<string, string>> item = ToTableItem(xItem).ToList();
                foreach (KeyValuePair<string, string> field in item)
                {
                    if (!table.Columns.Contains(field.Key))
                    {
                        table.Columns.Add(field.Key, typeof(string));
                    }
                }
                DataRow row = table.NewRow();
                foreach (KeyValuePair<string, string> field in item)
                {
                    row[field.Key] = field.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static IEnumerable<KeyValuePair<string, string>> ToTableItem(this XElement @this)
        {
            foreach (XAttribute attribute in @this.Attributes())
            {
                string key = attribute.Name.LocalName.Replace(Space, " ");
                string value = (string)attribute;
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}
