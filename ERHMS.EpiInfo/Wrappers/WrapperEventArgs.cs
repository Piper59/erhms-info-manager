using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ERHMS.EpiInfo.Wrappers
{
    public class WrapperEventArgs : EventArgs
    {
        internal const string Sentinel = "ERHMS";

        internal static string Serialize(string type, object properties)
        {
            return string.Format("{0} {1} {2}", Sentinel, type, DynamicExtensions.Serialize(properties));
        }

        internal static WrapperEventArgs Deserialize(string value)
        {
            IList<string> chunks = value.Split(new char[] { ' ' }, 3);
            if (chunks.Count < 3 || chunks[0] != Sentinel)
            {
                return null;
            }
            else
            {
                return new WrapperEventArgs
                {
                    Type = chunks[1],
                    properties = DynamicExtensions.Deserialize(chunks[2])
                };
            }
        }

        private ExpandoObject properties;

        public string Type { get; private set; }

        public dynamic Properties
        {
            get { return properties; }
        }

        private WrapperEventArgs() { }
    }
}
