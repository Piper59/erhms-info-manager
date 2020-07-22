using System;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public class FieldMappingException : Exception
    {
        public XField XField { get; }
        public string PropertyName { get; }

        public override string Message
        {
            get
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"An error occurred while mapping the '{PropertyName}' property.");
                message.Append(XField);
                return message.ToString();
            }
        }

        public FieldMappingException(XField xField, string propertyName, Exception inner)
            : base(null, inner)
        {
            XField = xField;
            PropertyName = propertyName;
        }
    }
}
