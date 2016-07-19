using Epi;

namespace ERHMS.EpiInfo
{
    public static class MetaFieldTypeExtensions
    {
        public static bool IsWebSupported(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.TextUppercase:
                case MetaFieldType.PhoneNumber:
                case MetaFieldType.DateTime:
                case MetaFieldType.CommandButton:
                case MetaFieldType.Image:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.Codes:
                case MetaFieldType.Relate:
                case MetaFieldType.GUID:
                    return false;
                default:
                    return true;
            }
        }

        public static bool IsMobileSupported(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.PhoneNumber:
                case MetaFieldType.DateTime:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.Codes:
                case MetaFieldType.Relate:
                case MetaFieldType.GUID:
                    return false;
                default:
                    return true;
            }
        }
    }
}
