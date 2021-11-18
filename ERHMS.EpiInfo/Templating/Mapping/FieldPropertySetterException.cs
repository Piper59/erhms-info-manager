using System;
using System.Runtime.Serialization;
using System.Text;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    [Serializable]
    public class FieldPropertySetterException : Exception
    {
        public string Xml { get; }
        public string PropertyName { get; }

        public override string Message
        {
            get
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"An error occurred while setting property '{PropertyName}'.");
                message.Append(Xml);
                return message.ToString();
            }
        }

        public FieldPropertySetterException(string xml, string propertyName, Exception innerException)
            : base(null, innerException)
        {
            Xml = xml;
            PropertyName = propertyName;
        }

        protected FieldPropertySetterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Xml = info.GetString(nameof(Xml));
            PropertyName = info.GetString(nameof(PropertyName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Xml), Xml, typeof(string));
            info.AddValue(nameof(PropertyName), PropertyName, typeof(string));
        }
    }
}
