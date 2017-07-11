using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ERHMS.EpiInfo.Wrappers
{
    public class WrapperEventArgs : EventArgs
    {
        internal static string Serialize(WrapperEventType type, object properties)
        {
            return string.Format("{0} {1}", type, DynamicExtensions.Serialize(properties));
        }

        internal static WrapperEventArgs Deserialize(string value)
        {
            IList<string> chunks = value.Split(new char[] { ' ' }, 2);
            return new WrapperEventArgs
            {
                Type = EnumExtensions.Parse<WrapperEventType>(chunks[0]),
                properties = DynamicExtensions.Deserialize(chunks[1])
            };
        }

        private ExpandoObject properties;

        public WrapperEventType Type { get; private set; }

        public dynamic Properties
        {
            get { return properties; }
        }
    }
}
