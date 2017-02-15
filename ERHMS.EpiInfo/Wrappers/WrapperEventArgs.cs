using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ERHMS.EpiInfo.Wrappers
{
    public class WrapperEventArgs : EventArgs
    {
        internal static string GetData(WrapperEventType type, object properties)
        {
            if (properties == null)
            {
                return type.ToString();
            }
            else
            {
                return string.Format("{0} {1}", type, DynamicExtensions.Serialize(properties));
            }
        }

        private ExpandoObject properties;

        public WrapperEventType Type { get; private set; }

        public dynamic Properties
        {
            get { return properties; }
        }

        internal WrapperEventArgs(string data)
        {
            IList<string> chunks = data.Split(new char[] { ' ' }, 2);
            Type = EnumExtensions.Parse<WrapperEventType>(chunks[0]);
            if (chunks.Count == 2)
            {
                properties = DynamicExtensions.Deserialize(chunks[1]);
            }
            else
            {
                properties = new ExpandoObject();
            }
        }
    }
}
