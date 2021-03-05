using Epi;
using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class ImageFieldMapper : FieldMapper<ImageField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Image;

        protected override FieldPropertySetterCollection<ImageField> PropertySetters { get; } =
            new FieldPropertySetterCollection<ImageField>
            {
                { field => field.ShouldRetainImageSize }
            };
    }
}
