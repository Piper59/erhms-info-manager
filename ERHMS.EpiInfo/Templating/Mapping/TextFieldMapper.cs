using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class TextFieldMapper : FieldMapper<TextField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Text;
        protected override FieldPropertySetterCollection<TextField> PropertySetters { get; } =
            new FieldPropertySetterCollection<TextField>
            {
                { field => field.MaxLength },
                { field => field.SourceFieldId },
                { field => field.IsEncrypted }
            };
    }
}
