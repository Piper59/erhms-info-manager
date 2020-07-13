using System;

namespace ERHMS.Utility
{
    public class InvalidEnumValueException : Exception
    {
        public Enum Value { get; }
        public Type EnumType => Value.GetType();
        public override string Message => $"The value '{Value}' is invalid for Enum type '{EnumType}.'";

        public InvalidEnumValueException(Enum value)
        {
            Value = value;
        }
    }
}
