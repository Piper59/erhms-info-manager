using Epi;

namespace ERHMS.EpiInfo
{
    public static class MetaFieldTypeExtensions
    {
        public static bool IsTextualData(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.LabelTitle:
                case MetaFieldType.CommandButton:
                case MetaFieldType.Image:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.Relate:
                case MetaFieldType.Group:
                case MetaFieldType.RecStatus:
                case MetaFieldType.UniqueKey:
                case MetaFieldType.ForeignKey:
                case MetaFieldType.GlobalRecordId:
                case MetaFieldType.UniqueRowId:
                    return false;
                default:
                    return true;
            }
        }

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
