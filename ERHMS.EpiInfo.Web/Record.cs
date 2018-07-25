using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ERHMS.EpiInfo.Web
{
    public class Record : Dictionary<string, string>
    {
        private static readonly ICollection<string> TrueValues = new string[]
        {
            "True",
            "1",
            "Yes"
        };
        private static readonly ICollection<string> FalseValues = new string[]
        {
            "False",
            "0",
            "No"
        };

        public string GlobalRecordId { get; set; }
        public string EntityId { get; set; }
        public string PassCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Record()
            : base(StringComparer.OrdinalIgnoreCase) { }

        public object GetValue(string key, Type type)
        {
            string value = this[key];
            if (type == typeof(string))
            {
                return value ?? "";
            }
            else if (type == typeof(bool))
            {
                if (TrueValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (FalseValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch
                {
                    return null;
                }
            }
        }

        public void SetValue(string key, object value)
        {
            if (value?.GetType() == typeof(bool))
            {
                value = (bool)value ? "Yes" : "No";
            }
            this[key] = value?.ToString();
        }

        public void SetValues(IEnumerable<KeyValuePair<string, object>> properties)
        {
            foreach (KeyValuePair<string, object> property in properties)
            {
                SetValue(property.Key, property.Value);
            }
        }

        public void SetValues(string xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                while (true)
                {
                    bool empty;
                    XmlElement element = reader.ReadNextElement(out empty);
                    if (element == null)
                    {
                        break;
                    }
                    if (element.Name == "ResponseDetail")
                    {
                        string key = element.GetAttribute("QuestionName");
                        string value = "";
                        if (!empty)
                        {
                            StringBuilder builder = new StringBuilder();
                            while (true)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == element.Name)
                                {
                                    break;
                                }
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    builder.Append(reader.Value);
                                }
                            }
                            value = builder.ToString();
                        }
                        this[key] = value;
                    }
                }
            }
        }

        public Uri GetUrl()
        {
            Uri endpoint = new Uri(Configuration.GetNewInstance().Settings.WebServiceEndpointAddress);
            return new Uri(endpoint, string.Format("Survey/{0}", GlobalRecordId));
        }
    }
}
