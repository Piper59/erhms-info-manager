using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class InputFieldWithoutSeparatePromptMapper : FieldMapper<InputFieldWithoutSeparatePrompt>
    {
        protected override MetaFieldType? FieldType => null;
        protected override FieldPropertySetterCollection<InputFieldWithoutSeparatePrompt> PropertySetters { get; } =
            new FieldPropertySetterCollection<InputFieldWithoutSeparatePrompt>
            {
                { field => field.ShouldRepeatLast },
                { field => field.IsRequired },
                { field => field.IsReadOnly }
            };
    }
}
