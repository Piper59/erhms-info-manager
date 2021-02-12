using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class RenderableFieldMapper : FieldMapper<RenderableField>
    {
        private static bool TryGetFont(XField xField, string propertyName, out Font font)
        {
            if (xField.TryGetAttribute($"{propertyName}Family", out XAttribute xFamily)
                && xField.TryGetAttribute($"{propertyName}Size", out XAttribute xSize)
                && xField.TryGetAttribute($"{propertyName}Style", out XAttribute xStyle))
            {
                FontFamily family = new FontFamily((string)xFamily);
                FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), (string)xStyle);
                font = new Font(family, (float)xSize, style);
                return true;
            }
            else
            {
                font = default;
                return false;
            }
        }

        private static bool TryGetControlFont(XField xField, out Font value)
        {
            return TryGetFont(xField, nameof(RenderableField.ControlFont), out value);
        }

        private static bool TryGetPromptFont(XField xField, out Font value)
        {
            return TryGetFont(xField, nameof(RenderableField.PromptFont), out value);
        }

        protected override MetaFieldType? FieldType => null;
        protected override FieldPropertySetterCollection<RenderableField> PropertySetters { get; } =
            new FieldPropertySetterCollection<RenderableField>
            {
                { field => field.PromptText },
                { field => field.ControlWidthPercentage },
                { field => field.ControlHeightPercentage },
                { field => field.ControlLeftPositionPercentage },
                { field => field.ControlTopPositionPercentage },
                { field => field.TabIndex },
                { field => field.HasTabStop },
                { field => field.ControlFont, TryGetControlFont },
                { field => field.PromptFont, TryGetPromptFont }
            };
    }
}
