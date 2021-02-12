using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class MirrorFieldMapper : FieldMapper<MirrorField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Mirror;
        protected override FieldPropertySetterCollection<MirrorField> PropertySetters { get; } =
            new FieldPropertySetterCollection<MirrorField>
            {
                { field => field.SourceFieldId }
            };

        public override bool MapProperties(MirrorField field)
        {
            bool changed = false;
            if (Context.MapFieldId(field.SourceFieldId, out int result))
            {
                field.SourceFieldId = result;
                changed = true;
            }
            return changed;
        }

        public override bool MapProperties(XField xField)
        {
            bool changed = false;
            if (xField.SourceFieldId != null && Context.MapFieldId(xField.SourceFieldId.Value, out int result))
            {
                xField.SourceFieldId = result;
                changed = true;
            }
            return changed;
        }
    }
}
