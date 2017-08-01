using ERHMS.Utility;

namespace ERHMS.EpiInfo
{
    public enum InvalidViewNameReason
    {
        None,
        Empty,
        InvalidChar,
        InvalidBeginning,
        TooLong,
        ViewExists,
        TableExists
    }

    public static class InvalidViewNameReasonExtensions
    {
        public static string GetErrorMessage(this InvalidViewNameReason @this)
        {
            switch (@this)
            {
                case InvalidViewNameReason.Empty:
                    return "Please enter a form name.";
                case InvalidViewNameReason.InvalidChar:
                    return "Please enter a form name that contains only letters, numbers, and underscores.";
                case InvalidViewNameReason.InvalidBeginning:
                    return "Please enter a form name that begins with a letter.";
                case InvalidViewNameReason.TooLong:
                    return "Please enter a form name that is no longer than 64 characters.";
                case InvalidViewNameReason.ViewExists:
                    return "This form name is already in use. Please enter a different form name.";
                case InvalidViewNameReason.TableExists:
                    return "A table with this name already exists in the database. Please enter a different form name.";
                default:
                    throw new InvalidEnumValueException(@this);
            }
        }
    }
}
