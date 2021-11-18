using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    [Serializable]
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

        protected FieldPropertySetterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            XField = new XField(XElement.Parse(info.GetString(nameof(XField))));
            PropertyName = info.GetString(nameof(PropertyName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(XField), XField.ToString(), typeof(string));
            info.AddValue(nameof(PropertyName), PropertyName, typeof(string));
        }
    }
}
