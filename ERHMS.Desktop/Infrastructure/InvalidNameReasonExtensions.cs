using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Naming;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    public static class InvalidNameReasonExtensions
    {
        public static string GetLead(this InvalidNameReason @this)
        {
            switch (@this)
            {
                case InvalidNameReason.Empty:
                    return Strings.InvalidNameReason_Lead_Empty;
                case InvalidNameReason.TooLong:
                    return Strings.InvalidNameReason_Lead_TooLong;
                case InvalidNameReason.InvalidChar:
                case InvalidNameReason.InvalidStartChar:
                case InvalidNameReason.Identical:
                case InvalidNameReason.Similar:
                    return Strings.InvalidNameReason_Lead_Default;
                case InvalidNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static string GetProjectBody(this InvalidNameReason @this)
        {
            switch (@this)
            {
                case InvalidNameReason.Empty:
                    return Strings.InvalidNameReason_Body_Project_Empty;
                case InvalidNameReason.TooLong:
                    return string.Format(
                        Strings.InvalidNameReason_Body_Project_TooLong,
                        ProjectNameValidator.MaxLength);
                case InvalidNameReason.InvalidChar:
                    return Strings.InvalidNameReason_Body_Project_InvalidChar;
                case InvalidNameReason.InvalidStartChar:
                    return Strings.InvalidNameReason_Body_Project_InvalidStartChar;
                case InvalidNameReason.Identical:
                    return Strings.InvalidNameReason_Body_Project_Identical;
                case InvalidNameReason.Similar:
                case InvalidNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static string GetViewBody(this InvalidNameReason @this)
        {
            switch (@this)
            {
                case InvalidNameReason.Empty:
                    return Strings.InvalidNameReason_Body_View_Empty;
                case InvalidNameReason.TooLong:
                    return string.Format(Strings.InvalidNameReason_Body_View_TooLong, ViewNameValidator.MaxLength);
                case InvalidNameReason.InvalidChar:
                    return Strings.InvalidNameReason_Body_View_InvalidChar;
                case InvalidNameReason.InvalidStartChar:
                    return Strings.InvalidNameReason_Body_View_InvalidStartChar;
                case InvalidNameReason.Identical:
                    return Strings.InvalidNameReason_Body_View_Identical;
                case InvalidNameReason.Similar:
                    return Strings.InvalidNameReason_Body_View_Similar;
                case InvalidNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
