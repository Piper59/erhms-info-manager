using System;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.Communication
{
    public class EventArgsBase : EventArgs
    {
        public string Tag { get; private set; }

        protected EventArgsBase(string tag = null)
        {
            Tag = tag;
        }

        protected string ToString(params string[] values)
        {
            StringBuilder builder = new StringBuilder();
            if (Tag != null)
            {
                builder.AppendFormat("({0})", Tag);
                if (values.Any())
                {
                    builder.Append(" ");
                }
            }
            if (values.Any())
            {
                builder.AppendFormat("{0}", string.Join(", ", values));
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            return ToString(new string[] { });
        }
    }
}
