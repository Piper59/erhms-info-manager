using Epi;

namespace ERHMS.EpiInfo
{
    public static class MetaFieldTypeExtensions
    {
        public static bool HasCodeTable(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.LegalValues:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.Codes:
                    return true;
                default:
                    return false;
            }
        }
    }
}
