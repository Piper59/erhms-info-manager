using System;

namespace ERHMS.Utility
{
    public class InvalidEnumValueException : Exception
    {
        public Enum Value { get; private set; }

        public override string Message
        {
            get { return string.Format("The value '{0}' is invalid for Enum type '{1}'.", Value, Value.GetType()); }
        }

        public InvalidEnumValueException(Enum value)
        {
            Value = value;
        }
    }
}
