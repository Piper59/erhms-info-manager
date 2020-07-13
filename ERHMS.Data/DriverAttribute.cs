using System;

namespace ERHMS.Data
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DriverAttribute : Attribute
    {
        public string Driver { get; }

        public DriverAttribute(string driver)
        {
            Driver = driver;
        }
    }
}
