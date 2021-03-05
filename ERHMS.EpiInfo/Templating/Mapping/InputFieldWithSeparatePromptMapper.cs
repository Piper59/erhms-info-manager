using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class InputFieldWithSeparatePromptMapper : FieldMapper<InputFieldWithSeparatePrompt>
    {
        protected override MetaFieldType? FieldType => null;

        protected override FieldPropertySetterCollection<InputFieldWithSeparatePrompt> PropertySetters { get; } =
            new FieldPropertySetterCollection<InputFieldWithSeparatePrompt>
            {
                { field => field.ShouldRepeatLast },
                { field => field.IsRequired },
                { field => field.IsReadOnly }
            };
    }
}
