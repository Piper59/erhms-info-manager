using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class FieldPropertySetterException : Exception
    {
        public XField XField { get; }
        public string PropertyName { get; }

        private readonly string message;
        public override string Message => message;

        public FieldPropertySetterException(XField xField, string propertyName, Exception innerException)
            : base(null, innerException)
        {
            XField = xField;
            PropertyName = propertyName;
            StringBuilder message = new StringBuilder();
            message.AppendLine($"An error occurred while setting property '{propertyName}'.");
            message.Append(xField);
            this.message = message.ToString();
        }
    }
}
