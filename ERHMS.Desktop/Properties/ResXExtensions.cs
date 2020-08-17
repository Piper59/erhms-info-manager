using ERHMS.EpiInfo;
using System;

namespace ERHMS.Desktop.Properties
{
    public static class ResXExtensions
    {
        public static string GetInvalidViewNameLead(InvalidViewNameReason reason)
        {
            switch (reason)
            {
                case InvalidViewNameReason.Empty:
                    return ResX.ViewNameEmptyLead;
                case InvalidViewNameReason.InvalidChar:
                case InvalidViewNameReason.InvalidBeginning:
                case InvalidViewNameReason.TooLong:
                case InvalidViewNameReason.Exists:
                case InvalidViewNameReason.IsConflict:
                    return ResX.ViewNameInvalidLead;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reason));
            }
        }

        public static string GetInvalidViewNameBody(InvalidViewNameReason reason)
        {
            switch (reason)
            {
                case InvalidViewNameReason.Empty:
                    return ResX.ViewNameEmptyBody;
                case InvalidViewNameReason.InvalidChar:
                    return ResX.ViewNameInvalidCharBody;
                case InvalidViewNameReason.InvalidBeginning:
                    return ResX.ViewNameInvalidBeginningBody;
                case InvalidViewNameReason.TooLong:
                    return string.Format(ResX.ViewNameTooLongBody, ViewNameGenerator.MaxLength);
                case InvalidViewNameReason.Exists:
                    return ResX.ViewNameExistsBody;
                case InvalidViewNameReason.IsConflict:
                    return ResX.ViewNameIsConflictBody;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reason));
            }
        }
    }
}
