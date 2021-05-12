using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class RenderableFieldMapper : FieldMapper<RenderableField>
    {
        private class FontAttributes
        {
            public XField XField { get; }
            public string PropertyName { get; }
            public FontFamily Family => new FontFamily((string)GetAttribute());
            public FontStyle Style => (FontStyle)Enum.Parse(typeof(FontStyle), (string)GetAttribute());
            public float Size => (float)GetAttribute();

            public FontAttributes(XField xField, string propertyName)
            {
                XField = xField;
                PropertyName = propertyName;
            }

            private string GetAttributeName(string suffix)
            {
                return $"{PropertyName}{suffix}";
            }

            private bool HasAttribute(string suffix)
            {
                return XField.TryGetAttribute(GetAttributeName(suffix), out _);
            }

            private XAttribute GetAttribute([CallerMemberName] string suffix = null)
            {
                return XField.Attribute(GetAttributeName(suffix));
            }

            public bool IsValid()
            {
                return HasAttribute(nameof(Family)) && HasAttribute(nameof(Style)) && HasAttribute(nameof(Size));
            }

            public Font GetFont()
            {
                return new Font(Family, Size, Style);
            }
        }

        private static bool TryGetFont(XField xField, string propertyName, out Font font)
        {
            FontAttributes attributes = new FontAttributes(xField, propertyName);
            if (attributes.IsValid())
            {
                font = attributes.GetFont();
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
        protected override FieldPropertySetterCollection<RenderableField> Setters { get; } =
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
