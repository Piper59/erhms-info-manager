using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ERHMS.EpiInfo
{
    public class WrapperEventArgs : EventArgs
    {
        internal static WrapperEventArgs Parse(string line)
        {
            IList<string> chunks = line.Split();
            WrapperEventType type = EnumExtensions.Parse<WrapperEventType>(chunks[0]);
            if (chunks.Count == 1)
            {
                return new WrapperEventArgs(type);
            }
            else
            {
                return new WrapperEventArgs(type, QueryString.Parse(chunks[1]));
            }
        }

        private ExpandoObject properties;

        public WrapperEventType Type { get; private set; }

        public dynamic Properties
        {
            get { return properties; }
        }

        internal WrapperEventArgs(WrapperEventType type)
        {
            Type = type;
            properties = new ExpandoObject();
        }

        internal WrapperEventArgs(WrapperEventType type, IEnumerable<KeyValuePair<string, string>> properties)
            : this(type)
        {
            foreach (KeyValuePair<string, string> property in properties)
            {
                ((IDictionary<string, object>)this.properties).Add(property.Key, property.Value);
            }
        }
    }
}
