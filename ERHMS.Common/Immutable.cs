using System;

namespace ERHMS.Common
{
    public class Immutable<TValue>
    {
        public bool HasValue { get; private set; }

        private TValue value;
        public TValue Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Immutable object has no value.");
                }
                return value;
            }
            set
            {
                if (HasValue)
                {
                    throw new InvalidOperationException("Immutable object already has a value.");
                }
                this.value = value;
                HasValue = true;
            }
        }
    }
}
