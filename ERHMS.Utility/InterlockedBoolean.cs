using System;
using System.Threading;

namespace ERHMS.Utility
{
    public class InterlockedBoolean
    {
        private int value;

        public bool Value
        {
            get { return Convert.ToBoolean(value); }
        }

        public InterlockedBoolean(bool value)
        {
            this.value = Convert.ToInt32(value);
        }

        public bool Exchange(bool value)
        {
            return Convert.ToBoolean(Interlocked.Exchange(ref this.value, Convert.ToInt32(value)));
        }

        public bool CompareExchange(bool value, bool comparand)
        {
            return Convert.ToBoolean(Interlocked.CompareExchange(ref this.value, Convert.ToInt32(value), Convert.ToInt32(comparand)));
        }
    }
}
