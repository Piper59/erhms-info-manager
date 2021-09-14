using System;

namespace ERHMS.EpiInfo
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
