using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class PhoneNumberFieldMapper : FieldMapper<PhoneNumberField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.PhoneNumber;
        protected override FieldPropertySetterCollection<PhoneNumberField> PropertySetters { get; } =
            new FieldPropertySetterCollection<PhoneNumberField>
            {
                { field => field.Pattern }
            };
    }
}
