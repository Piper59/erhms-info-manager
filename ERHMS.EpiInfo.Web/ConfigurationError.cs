using System;

namespace ERHMS.EpiInfo.Web
{
    public enum ConfigurationError
    {
        None,
        Version,
        OrganizationKey,
        Connection,
        Unknown
    }

    public static class ConfigurationErrorExtensions
    {
        public static string GetMessage(this ConfigurationError @this)
        {
            switch (@this)
            {
                case ConfigurationError.Version:
                    return "Invalid endpoint address (version 2 required).";
                case ConfigurationError.OrganizationKey:
                    return "Invalid organization key.";
                case ConfigurationError.Connection:
                    return "Failed to connect to endpoint.";
                case ConfigurationError.Unknown:
                    return "Configuration error.";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
