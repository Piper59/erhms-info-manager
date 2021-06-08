using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Naming;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    public static class InvalidViewNameReasonExtensions
    {
        public static string GetLead(this InvalidViewNameReason @this)
        {
            switch (@this)
            {
                case InvalidViewNameReason.Empty:
                    return ResXResources.InvalidViewNameReason_Lead_Empty;
                case InvalidViewNameReason.TooLong:
                    return ResXResources.InvalidViewNameReason_Lead_TooLong;
                case InvalidViewNameReason.InvalidChar:
                case InvalidViewNameReason.InvalidStartChar:
                case InvalidViewNameReason.Identical:
                case InvalidViewNameReason.Similar:
                    return ResXResources.InvalidViewNameReason_Lead;
                case InvalidViewNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static string GetBody(this InvalidViewNameReason @this)
        {
            switch (@this)
            {
                case InvalidViewNameReason.Empty:
                    return ResXResources.InvalidViewNameReason_Body_Empty;
                case InvalidViewNameReason.TooLong:
                    return string.Format(ResXResources.InvalidViewNameReason_Body_TooLong, ViewNameValidator.MaxLength);
                case InvalidViewNameReason.InvalidChar:
                    return ResXResources.InvalidViewNameReason_Body_InvalidChar;
                case InvalidViewNameReason.InvalidStartChar:
                    return ResXResources.InvalidViewNameReason_Body_InvalidStartChar;
                case InvalidViewNameReason.Identical:
                    return ResXResources.InvalidViewNameReason_Body_Identical;
                case InvalidViewNameReason.Similar:
                    return ResXResources.InvalidViewNameReason_Body_Similar;
                case InvalidViewNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
