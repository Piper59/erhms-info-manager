using System;

namespace ERHMS.EpiInfo
{
    [Serializable]
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
