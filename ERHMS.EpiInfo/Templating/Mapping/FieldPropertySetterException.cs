using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class FieldPropertySetterException : Exception
    {
        public XField XField { get; }
        public string PropertyName { get; }

        public override string Message
        {
            get
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"An error occurred while setting property '{PropertyName}'.");
                message.Append(XField);
                return message.ToString();
            }
        }

        public FieldPropertySetterException(XField xField, string propertyName, Exception innerException)
            : base(null, innerException)
        {
            XField = xField;
            PropertyName = propertyName;
        }
    }
}
