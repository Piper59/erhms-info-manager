using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Naming;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    public static class InvalidNameReasonExtensions
    {
        public static string GetProjectLead(this InvalidNameReason @this)
        {
            switch (@this)
            {
                case InvalidNameReason.Empty:
                    return Strings.InvalidNameReason_Project_Lead_Empty;
                case InvalidNameReason.TooLong:
                    return Strings.InvalidNameReason_Project_Lead_TooLong;
                case InvalidNameReason.InvalidChar:
                case InvalidNameReason.InvalidStartChar:
                case InvalidNameReason.Identical:
                    return Strings.InvalidNameReason_Project_Lead_Default;
                case InvalidNameReason.Similar:
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
                    return Strings.InvalidNameReason_Project_Body_Empty;
                case InvalidNameReason.TooLong:
                    return string.Format(
                        Strings.InvalidNameReason_Project_Body_TooLong,
                        ProjectNameValidator.MaxLength);
                case InvalidNameReason.InvalidChar:
                    return Strings.InvalidNameReason_Project_Body_InvalidChar;
                case InvalidNameReason.InvalidStartChar:
                    return Strings.InvalidNameReason_Project_Body_InvalidStartChar;
                case InvalidNameReason.Identical:
                    return Strings.InvalidNameReason_Project_Body_Identical;
                case InvalidNameReason.Similar:
                case InvalidNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static string GetViewLead(this InvalidNameReason @this)
        {
            switch (@this)
            {
                case InvalidNameReason.Empty:
                    return Strings.InvalidNameReason_View_Lead_Empty;
                case InvalidNameReason.TooLong:
                    return Strings.InvalidNameReason_View_Lead_TooLong;
                case InvalidNameReason.InvalidChar:
                case InvalidNameReason.InvalidStartChar:
                case InvalidNameReason.Identical:
                case InvalidNameReason.Similar:
                    return Strings.InvalidNameReason_View_Lead_Default;
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
                    return Strings.InvalidNameReason_View_Body_Empty;
                case InvalidNameReason.TooLong:
                    return string.Format(Strings.InvalidNameReason_View_Body_TooLong, ViewNameValidator.MaxLength);
                case InvalidNameReason.InvalidChar:
                    return Strings.InvalidNameReason_View_Body_InvalidChar;
                case InvalidNameReason.InvalidStartChar:
                    return Strings.InvalidNameReason_View_Body_InvalidStartChar;
                case InvalidNameReason.Identical:
                    return Strings.InvalidNameReason_View_Body_Identical;
                case InvalidNameReason.Similar:
                    return Strings.InvalidNameReason_View_Body_Similar;
                case InvalidNameReason.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
