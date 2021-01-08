using Epi;
using System;

namespace ERHMS.EpiInfo.Metadata
{
    public static class MetaFieldTypeExtensions
    {
        public static bool IsTableBased(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.LegalValues:
                case MetaFieldType.Codes:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.List:
                    return true;
                case MetaFieldType.Text:
                case MetaFieldType.LabelTitle:
                case MetaFieldType.TextUppercase:
                case MetaFieldType.Multiline:
                case MetaFieldType.Number:
                case MetaFieldType.PhoneNumber:
                case MetaFieldType.Date:
                case MetaFieldType.Time:
                case MetaFieldType.DateTime:
                case MetaFieldType.Checkbox:
                case MetaFieldType.YesNo:
                case MetaFieldType.Option:
                case MetaFieldType.CommandButton:
                case MetaFieldType.Image:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.Relate:
                case MetaFieldType.Group:
                case MetaFieldType.RecStatus:
                case MetaFieldType.UniqueKey:
                case MetaFieldType.ForeignKey:
                case MetaFieldType.GUID:
                case MetaFieldType.GlobalRecordId:
                case MetaFieldType.UniqueRowId:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static bool IsTextualData(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.Text:
                case MetaFieldType.TextUppercase:
                case MetaFieldType.Multiline:
                case MetaFieldType.Number:
                case MetaFieldType.PhoneNumber:
                case MetaFieldType.Date:
                case MetaFieldType.Time:
                case MetaFieldType.DateTime:
                case MetaFieldType.Checkbox:
                case MetaFieldType.YesNo:
                case MetaFieldType.Option:
                case MetaFieldType.LegalValues:
                case MetaFieldType.Codes:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.GUID:
                case MetaFieldType.List:
                    return true;
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
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
