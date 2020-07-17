using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public class FieldMapper<TField> : IFieldMapper
        where TField : Field
    {
        protected FieldMappingCollection<TField> Mappings { get; set; }

        protected virtual void OnError(FieldMappingException ex)
        {
            Log.Default.Warn(ex);
        }

        public void SetProperties(XField xField, TField field)
        {
            foreach (FieldMapping<TField> mapping in Mappings)
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
        private static Font GetFont(XField xField, string propertyName)
        {
            ICollection<string> keys = new string[] {
                "Family",
                "Size",
                "Style"
            };
            IDictionary<string, string> attributes = keys.ToDictionary(
                key => key,
                key => xField.Attribute($"{propertyName}{key}").Value);
            if (attributes.Values.Any(value => value == ""))
            {
                return null;
            }
            FontFamily family = new FontFamily(attributes["Family"]);
            float size = float.Parse(attributes["Size"]);
            FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), attributes["Style"]);
            return new Font(family, size, style);
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
                { f => f.ControlFont, xf => GetFont(xf, nameof(RenderableField.ControlFont)) },
                { f => f.PromptFont, xf => GetFont(xf, nameof(RenderableField.PromptFont)) }
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

        private static List<string> GetOptions(XField xField)
        {
            string options = (string)xField.Attribute(ColumnNames.LIST);
            int index = options.IndexOf(OptionsSeparator);
            if (index != -1)
            {
                options = options.Substring(0, index);
            }
            return options.Split(Constants.LIST_SEPARATOR).ToList();
        }

        public OptionFieldMapper()
        {
            Mappings = new FieldMappingCollection<OptionField>
            {
                { f => f.Pattern },
                { f => f.ShowTextOnRight },
                { f => f.Options, xf => GetOptions(xf) }
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
        private static Color GetBackgroundColor(XField xField)
        {
            return Color.FromArgb((int)xField.Attribute(ColumnNames.BACKGROUND_COLOR));
        }

        public GroupFieldMapper()
        {
            Mappings = new FieldMappingCollection<GroupField>
            {
                { f => f.ChildFieldNames, ColumnNames.LIST },
                { f => f.BackgroundColor, xf => GetBackgroundColor(xf) }
            };
        }
    }
}
