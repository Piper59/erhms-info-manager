using ERHMS.Utility;

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
        public static string GetErrorMessage(this ConfigurationError @this)
        {
            switch (@this)
            {
                case ConfigurationError.Address:
                    return "Invalid endpoint address.";
                case ConfigurationError.Version:
                    return "Endpoint address must be version 2 (SurveyManagerServiceV2.svc) or later.";
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
