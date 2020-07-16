using Epi;

namespace ERHMS.EpiInfo
{
    public static class MetaFieldTypeExtensions
    {
        public static bool IsTableBased(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.LegalValues:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.Codes:
                case MetaFieldType.List:
                    return true;
                default:
                    return false;
            }
        }
    }
}
