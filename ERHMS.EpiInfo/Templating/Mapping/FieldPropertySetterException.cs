using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class FieldPropertySetterException : Exception
    {
        private static string GetMessage(XField xField, string propertyName)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine($"An error occurred while setting property '{propertyName}'.");
            message.Append(xField);
            return message.ToString();
        }

        public XField XField { get; }
        public string PropertyName { get; }

        public FieldPropertySetterException(XField xField, string propertyName, Exception innerException)
            : base(GetMessage(xField, propertyName), innerException)
        {
            XField = xField;
            PropertyName = propertyName;
        }
    }
}
