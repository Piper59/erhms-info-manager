using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class DateFieldMapper : FieldMapper<DateField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Date;

        protected override FieldPropertySetterCollection<DateField> PropertySetters { get; } =
            new FieldPropertySetterCollection<DateField>
            {
                { field => field.Lower },
                { field => field.Upper }
            };
    }
}
