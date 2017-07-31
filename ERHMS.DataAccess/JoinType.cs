using ERHMS.Utility;

namespace ERHMS.DataAccess
{
    public enum JoinType
    {
        Inner,
        LeftOuter
    }

    public static class JoinTypeExtensions
    {
        public static string ToSql(this JoinType @this)
        {
            switch (@this)
            {
                case JoinType.Inner:
                    return "INNER";
                case JoinType.LeftOuter:
                    return "LEFT OUTER";
                default:
                    throw new InvalidEnumValueException(@this);
            }
        }
    }
}
