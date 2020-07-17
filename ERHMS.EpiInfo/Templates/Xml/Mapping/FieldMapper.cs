using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public class FieldMapper<TField> : IFieldMapper<TField>
        where TField : Field
    {
        protected FieldMappingCollection<TField> Mappings { get; set; }

        protected virtual void OnError(FieldMappingException ex)
        {
            Log.Default.Warn(ex);
        }

        public void SetProperties(XField xField, TField field)
        {
            foreach (IFieldMapping<TField> mapping in Mappings)
            {
                try
                {
                    mapping.SetProperty(xField, field);
                }
                catch (FieldMappingException ex)
                {
                    OnError(ex);
                }
            }
        }

        public void SetProperties(XField xField, Field field)
        {
            if (field is TField typedField)
            {
                SetProperties(xField, typedField);
            }
        }
    }
    public class RenderableFieldMapper : FieldMapper<RenderableField>
    {
        private static bool TryGetFont(XField xField, string propertyName, out Font value)
        {
            ICollection<string> keys = new string[] {
                "Family",
                "Size",
                "Style"
            };
            IDictionary<string, XAttribute> attributes = keys.ToDictionary(
                key => key,
                key => xField.Attribute($"{propertyName}{key}"));
            if (attributes.Values.Any(attribute => attribute == null || attribute.Value == ""))
            {
                value = null;
                return false;
            }
            FontFamily family = new FontFamily((string)attributes["Family"]);
            float size = (float)attributes["Size"];
            FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), (string)attributes["Style"]);
            value = new Font(family, size, style);
            return true;
        }

        private static bool TryGetControlFont(XField xField, out Font value)
        {
            return TryGetFont(xField, nameof(RenderableField.ControlFont), out value);
        }

        private static bool TryGetPromptFont(XField xField, out Font value)
        {
            return TryGetFont(xField, nameof(RenderableField.PromptFont), out value);
        }

        public RenderableFieldMapper()
        {
            Mappings = new FieldMappingCollection<RenderableField>
            {
                { f => f.PromptText },
                { f => f.ControlWidthPercentage },
                { f => f.ControlHeightPercentage },
                { f => f.ControlLeftPositionPercentage },
                { f => f.ControlTopPositionPercentage },
                { f => f.TabIndex },
                { f => f.HasTabStop },
                { f => f.ControlFont, TryGetControlFont },
                { f => f.PromptFont, TryGetPromptFont }
            };
        }
    }

    public class FieldWithSeparatePromptMapper : FieldMapper<FieldWithSeparatePrompt>
    {
        public FieldWithSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<FieldWithSeparatePrompt>
            {
                { f => f.PromptLeftPositionPercentage },
                { f => f.PromptTopPositionPercentage }
            };
        }
    }

    public class InputFieldWithoutSeparatePromptMapper : FieldMapper<InputFieldWithoutSeparatePrompt>
    {
        public InputFieldWithoutSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<InputFieldWithoutSeparatePrompt>
            {
                { f => f.ShouldRepeatLast },
                { f => f.IsRequired },
                { f => f.IsReadOnly }
            };
        }
    }

    public class InputFieldWithSeparatePromptMapper : FieldMapper<InputFieldWithSeparatePrompt>
    {
        public InputFieldWithSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<InputFieldWithSeparatePrompt>
            {
                { f => f.ShouldRepeatLast },
                { f => f.IsRequired },
                { f => f.IsReadOnly }
            };
        }
    }

    public class TextFieldMapper : FieldMapper<TextField>
    {
        public TextFieldMapper()
        {
            Mappings = new FieldMappingCollection<TextField>
            {
                { f => f.MaxLength },
                { f => f.SourceFieldId },
                { f => f.IsEncrypted }
            };
        }
    }

    public class NumberFieldMapper : FieldMapper<NumberField>
    {
        public NumberFieldMapper()
        {
            Mappings = new FieldMappingCollection<NumberField>
            {
                { f => f.Pattern },
                { f => f.Lower },
                { f => f.Upper }
            };
        }
    }

    public class PhoneNumberFieldMapper : FieldMapper<PhoneNumberField>
    {
        public PhoneNumberFieldMapper()
        {
            Mappings = new FieldMappingCollection<PhoneNumberField>
            {
                { f => f.Pattern }
            };
        }
    }

    public class DateFieldMapper : FieldMapper<DateField>
    {
        public DateFieldMapper()
        {
            Mappings = new FieldMappingCollection<DateField>
            {
                { f => f.Lower },
                { f => f.Upper }
            };
        }
    }

    public class OptionFieldMapper : FieldMapper<OptionField>
    {
        private const string OptionsSeparator = "||";

        private static bool TryGetOptions(XField xField, out List<string> value)
        {
            string options = (string)xField.Attribute(ColumnNames.LIST);
            int index = options.IndexOf(OptionsSeparator);
            if (index != -1)
            {
                options = options.Substring(0, index);
            }
            value = options.Split(Constants.LIST_SEPARATOR).ToList();
            return true;
        }

        public OptionFieldMapper()
        {
            Mappings = new FieldMappingCollection<OptionField>
            {
                { f => f.Pattern },
                { f => f.ShowTextOnRight },
                { f => f.Options, TryGetOptions }
            };
        }
    }

    public class ImageFieldMapper : FieldMapper<ImageField>
    {
        public ImageFieldMapper()
        {
            Mappings = new FieldMappingCollection<ImageField>
            {
                { f => f.ShouldRetainImageSize }
            };
        }
    }

    public class MirrorFieldMapper : FieldMapper<MirrorField>
    {
        public MirrorFieldMapper()
        {
            Mappings = new FieldMappingCollection<MirrorField>
            {
                { f => f.SourceFieldId }
            };
        }
    }

    public class TableBasedDropDownFieldMapper : FieldMapper<TableBasedDropDownField>
    {
        public TableBasedDropDownFieldMapper()
        {
            Mappings = new FieldMappingCollection<TableBasedDropDownField>
            {
                { f => f.ShouldSort, ColumnNames.SORT },
                { f => f.TextColumnName },
                { f => f.CodeColumnName },
                { f => f.SourceTableName },
                { f => f.IsExclusiveTable }
            };
        }
    }

    public class DDLFieldOfCodesMapper : FieldMapper<DDLFieldOfCodes>
    {
        public DDLFieldOfCodesMapper()
        {
            Mappings = new FieldMappingCollection<DDLFieldOfCodes>
            {
                { f => f.AssociatedFieldInformation, ColumnNames.RELATE_CONDITION }
            };
        }
    }

    public class RelatedViewFieldMapper : FieldMapper<RelatedViewField>
    {
        public RelatedViewFieldMapper()
        {
            Mappings = new FieldMappingCollection<RelatedViewField>
            {
                { f => f.RelatedViewID, "RelatedViewId" },
                { f => f.ShouldReturnToParent },
                { f => f.Condition, ColumnNames.RELATE_CONDITION }
            };
        }
    }

    public class GroupFieldMapper : FieldMapper<GroupField>
    {
        private static bool GetBackgroundColor(XField xField, out Color value)
        {
            XAttribute attribute = xField.Attribute(ColumnNames.BACKGROUND_COLOR);
            if (attribute == null || attribute.Value == "")
            {
                value = Color.Empty;
                return false;
            }
            value = Color.FromArgb((int)attribute);
            return true;
        }

        public GroupFieldMapper()
        {
            Mappings = new FieldMappingCollection<GroupField>
            {
                { f => f.ChildFieldNames, ColumnNames.LIST },
                { f => f.BackgroundColor, GetBackgroundColor }
            };
        }
    }
}
