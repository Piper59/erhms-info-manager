using ERHMS.Utility;
using System;

namespace ERHMS.EpiInfo.Web
{
    public enum ConfigurationError
    {
        None,
        Address,
        Version,
        OrganizationKey,
        Connection,
        Unknown
    }

    public static class ConfigurationErrorExtensions
    {
        public static string GetErrorMessage(this ConfigurationError @this, int? version = null)
        {
            switch (@this)
            {
                case ConfigurationError.Address:
                    return "Invalid endpoint address.";
                case ConfigurationError.Version:
                    if (!version.HasValue)
                    {
                        throw new ArgumentNullException(nameof(version));
                    }
                    return string.Format("Endpoint address must be version {0} (SurveyManagerServiceV{0}.svc) or later.", version);
                case ConfigurationError.OrganizationKey:
                    return "Invalid organization key.";
                case ConfigurationError.Connection:
                    return "Failed to connect to service.";
                case ConfigurationError.Unknown:
                    return "Configuration error.";
                default:
                    throw new InvalidEnumValueException(@this);
            }
        }
    }
}
