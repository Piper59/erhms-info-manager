using ERHMS.Utility;

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
                    return "Version 2 endpoint address (SurveyManagerServiceV2.svc) required.";
                case ConfigurationError.OrganizationKey:
                    return "Invalid organization key.";
                case ConfigurationError.Connection:
                    return "Failed to connect to endpoint.";
                case ConfigurationError.Unknown:
                    return "Configuration error.";
                default:
                    throw new InvalidEnumValueException(@this);
            }
        }
    }
}
