using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class NumberFieldMapper : FieldMapper<NumberField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Number;

        protected override FieldPropertySetterCollection<NumberField> PropertySetters { get; } =
            new FieldPropertySetterCollection<NumberField>
            {
                { field => field.Pattern },
                { field => field.Lower },
                { field => field.Upper }
            };
    }
}
