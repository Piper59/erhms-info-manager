using Epi;
using Epi.Fields;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public class FieldMapper<TField> : IFieldMapper<TField>
        where TField : Field
    {
        protected FieldMappingCollection<TField> Mappings { get; set; }

        protected void OnError(FieldMappingException ex)
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

    public class DDLFieldOfCodesMapper : FieldMapper<DDLFieldOfCodes>
    {
        private const char FieldInfoSeparator = ':';

        public static string MapAssociatedFieldInformation(string value, IDictionary<int, int> fieldIdMap)
        {
            IList<string> fieldInfos = value.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < fieldInfos.Count; index++)
            {
                string fieldInfo = fieldInfos[index];
                IList<string> chunks = fieldInfo.Split(FieldInfoSeparator);
                if (chunks.Count != 2)
                {
                    continue;
                }
                string columnName = chunks[0];
                if (!int.TryParse(chunks[1], out int fieldId))
                {
                    continue;
                }
                if (!fieldIdMap.TryGetValue(fieldId, out fieldId))
                {
                    continue;
                }
                fieldInfos[index] = $"{columnName}{FieldInfoSeparator}{fieldId}";
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), fieldInfos);
        }

        public static void MapAssociatedFieldInformation(DDLFieldOfCodes field, IDictionary<int, int> fieldIdMap)
        {
            field.AssociatedFieldInformation = MapAssociatedFieldInformation(field.AssociatedFieldInformation, fieldIdMap);
        }

        public DDLFieldOfCodesMapper()
        {
            Mappings = new FieldMappingCollection<DDLFieldOfCodes>
            {
                { f => f.AssociatedFieldInformation, ColumnNames.RELATE_CONDITION }
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

    public class GroupFieldMapper : FieldMapper<GroupField>
    {
        private static bool TryGetBackgroundColor(XField xField, out Color value)
        {
            if (xField.BackgroundColor == null)
            {
                value = Color.Empty;
                return false;
            }
            else
            {
                value = Color.FromArgb(xField.BackgroundColor.Value);
                return true;
            }
        }

        public static string MapChildFieldNames(string value, IDictionary<string, string> fieldNameMap)
        {
            IList<string> fieldNames = value.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < fieldNames.Count; index++)
            {
                string original = fieldNames[index];
                if (fieldNameMap.TryGetValue(original, out string modified))
                {
                    fieldNames[index] = modified;
                }
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), fieldNames);
        }

        public static void MapChildFieldNames(GroupField field, IDictionary<string, string> fieldNameMap)
        {
            field.ChildFieldNames = MapChildFieldNames(field.ChildFieldNames, fieldNameMap);
        }

        public GroupFieldMapper()
        {
            Mappings = new FieldMappingCollection<GroupField>
            {
                { f => f.ChildFieldNames, ColumnNames.LIST },
                { f => f.BackgroundColor, TryGetBackgroundColor }
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

    public class OptionFieldMapper : FieldMapper<OptionField>
    {
        private const string OptionsSeparator = "||";

        private static bool TryGetOptions(XField xField, out List<string> value)
        {
            string options = xField.List;
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

    public class RenderableFieldMapper : FieldMapper<RenderableField>
    {
        private static bool TryGetControlFont(XField xField, out Font value)
        {
            return xField.TryGetFont(nameof(RenderableField.ControlFont), out value);
        }

        private static bool TryGetPromptFont(XField xField, out Font value)
        {
            return xField.TryGetFont(nameof(RenderableField.PromptFont), out value);
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
}
