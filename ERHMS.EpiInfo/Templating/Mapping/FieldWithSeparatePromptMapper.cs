using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class FieldWithSeparatePromptMapper : FieldMapper<FieldWithSeparatePrompt>
    {
        protected override MetaFieldType? FieldType => null;
        protected override FieldPropertySetterCollection<FieldWithSeparatePrompt> Setters { get; } =
            new FieldPropertySetterCollection<FieldWithSeparatePrompt>
            {
                { field => field.PromptLeftPositionPercentage },
                { field => field.PromptTopPositionPercentage }
            };
    }
}
