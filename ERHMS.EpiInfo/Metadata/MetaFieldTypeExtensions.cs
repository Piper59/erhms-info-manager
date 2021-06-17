using Epi;
using System;

namespace ERHMS.EpiInfo.Metadata
{
    public static class MetaFieldTypeExtensions
    {
        public static bool IsMetadata(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.RecStatus:
                case MetaFieldType.UniqueKey:
                case MetaFieldType.ForeignKey:
                case MetaFieldType.GlobalRecordId:
                case MetaFieldType.UniqueRowId:
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
                case MetaFieldType.LegalValues:
                case MetaFieldType.Codes:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.Relate:
                case MetaFieldType.Group:
                case MetaFieldType.GUID:
                case MetaFieldType.List:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static bool IsNumeric(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.Number:
                case MetaFieldType.Date:
                case MetaFieldType.Time:
                case MetaFieldType.DateTime:
                    return true;
                case MetaFieldType.Text:
                case MetaFieldType.LabelTitle:
                case MetaFieldType.TextUppercase:
                case MetaFieldType.Multiline:
                case MetaFieldType.PhoneNumber:
                case MetaFieldType.Checkbox:
                case MetaFieldType.YesNo:
                case MetaFieldType.Option:
                case MetaFieldType.CommandButton:
                case MetaFieldType.Image:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.LegalValues:
                case MetaFieldType.Codes:
                case MetaFieldType.CommentLegal:
                case MetaFieldType.Relate:
                case MetaFieldType.Group:
                case MetaFieldType.RecStatus:
                case MetaFieldType.UniqueKey:
                case MetaFieldType.ForeignKey:
                case MetaFieldType.GUID:
                case MetaFieldType.GlobalRecordId:
                case MetaFieldType.List:
                case MetaFieldType.UniqueRowId:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static bool IsPrintable(this MetaFieldType @this)
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
                case MetaFieldType.RecStatus:
                case MetaFieldType.UniqueKey:
                case MetaFieldType.ForeignKey:
                case MetaFieldType.GUID:
                case MetaFieldType.GlobalRecordId:
                case MetaFieldType.List:
                case MetaFieldType.UniqueRowId:
                    return true;
                case MetaFieldType.LabelTitle:
                case MetaFieldType.CommandButton:
                case MetaFieldType.Image:
                case MetaFieldType.Mirror:
                case MetaFieldType.Grid:
                case MetaFieldType.Relate:
                case MetaFieldType.Group:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

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

        public static Type ToClrType(this MetaFieldType @this)
        {
            switch (@this)
            {
                case MetaFieldType.Text:
                    return typeof(string);
                case MetaFieldType.LabelTitle:
                    return typeof(void);
                case MetaFieldType.TextUppercase:
                    return typeof(string);
                case MetaFieldType.Multiline:
                    return typeof(string);
                case MetaFieldType.Number:
                    return typeof(double?);
                case MetaFieldType.PhoneNumber:
                    return typeof(string);
                case MetaFieldType.Date:
                    return typeof(DateTime?);
                case MetaFieldType.Time:
                    return typeof(DateTime?);
                case MetaFieldType.DateTime:
                    return typeof(DateTime?);
                case MetaFieldType.Checkbox:
                    return typeof(bool?);
                case MetaFieldType.YesNo:
                    return typeof(byte?);
                case MetaFieldType.Option:
                    return typeof(short?);
                case MetaFieldType.CommandButton:
                    return typeof(void);
                case MetaFieldType.Image:
                    return typeof(byte[]);
                case MetaFieldType.Mirror:
                    return typeof(void);
                case MetaFieldType.Grid:
                    return typeof(void);
                case MetaFieldType.LegalValues:
                    return typeof(string);
                case MetaFieldType.Codes:
                    return typeof(string);
                case MetaFieldType.CommentLegal:
                    return typeof(string);
                case MetaFieldType.Relate:
                    return typeof(void);
                case MetaFieldType.Group:
                    return typeof(void);
                case MetaFieldType.RecStatus:
                    return typeof(short?);
                case MetaFieldType.UniqueKey:
                    return typeof(int?);
                case MetaFieldType.ForeignKey:
                    return typeof(string);
                case MetaFieldType.GUID:
                    return typeof(Guid?);
                case MetaFieldType.GlobalRecordId:
                    return typeof(string);
                case MetaFieldType.List:
                    return typeof(string);
                case MetaFieldType.UniqueRowId:
                    return typeof(string);
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
