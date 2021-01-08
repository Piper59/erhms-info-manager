using System;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public class FieldPropertyMapperException : Exception
    {
        private static string GetMessage(XField xField, string propertyName)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine($"An error occurred while mapping the '{propertyName}' property.");
            message.Append(xField);
            return message.ToString();
        }

        public XField XField { get; }
        public string PropertyName { get; }

        public FieldPropertyMapperException(XField xField, string propertyName, Exception innerException)
            : base(GetMessage(xField, propertyName), innerException)
        {
            XField = xField;
            PropertyName = propertyName;
        }
    }
}
