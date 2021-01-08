using Epi;
using Epi.Fields;
using ERHMS.Common.Logging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public delegate bool FieldIdMapper(int fieldId, out int result);
    public delegate bool FieldNameMapper(string fieldName, out string result);

    public abstract class FieldMapper<TField> : IFieldMapper<TField>
        where TField : Field
    {
        protected abstract FieldPropertyMapperCollection<TField> PropertyMappers { get; }

        public void SetProperties(XField xField, TField field)
        {
            foreach (IFieldPropertyMapper<TField> propertyMapper in PropertyMappers)
            {
                try
                {
                    propertyMapper.SetProperty(xField, field);
                }
                catch (FieldPropertyMapperException ex)
                {
                    Log.Instance.Warn(ex);
                }
            }
        }

        public bool TrySetProperties(XField xField, Field field)
        {
            if (field is TField typedField)
            {
                SetProperties(xField, typedField);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class DateFieldMapper : FieldMapper<DateField>
    {
        protected override FieldPropertyMapperCollection<DateField> PropertyMappers { get; } = new FieldPropertyMapperCollection<DateField>
        {
            { f => f.Lower },
            { f => f.Upper }
        };
    }

    public class DDLFieldOfCodesMapper : FieldMapper<DDLFieldOfCodes>
    {
        private const char FieldInfoSeparator = ':';

        public static string MapAssociatedFieldInformation(string value, FieldIdMapper mapper)
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
                if (!mapper(fieldId, out int mappedFieldId))
                {
                    continue;
                }
                string mappedFieldInfo = $"{columnName}{FieldInfoSeparator}{mappedFieldId}";
                fieldInfos[index] = mappedFieldInfo;
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), fieldInfos);
        }

        protected override FieldPropertyMapperCollection<DDLFieldOfCodes> PropertyMappers { get; } = new FieldPropertyMapperCollection<DDLFieldOfCodes>
        {
            { f => f.AssociatedFieldInformation, ColumnNames.RELATE_CONDITION }
        };
    }

    public class FieldWithSeparatePromptMapper : FieldMapper<FieldWithSeparatePrompt>
    {
        protected override FieldPropertyMapperCollection<FieldWithSeparatePrompt> PropertyMappers { get; } = new FieldPropertyMapperCollection<FieldWithSeparatePrompt>
        {
            { f => f.PromptLeftPositionPercentage },
            { f => f.PromptTopPositionPercentage }
        };
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

        public static string MapChildFieldNames(string value, FieldNameMapper mapper)
        {
            IList<string> fieldNames = value.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < fieldNames.Count; index++)
            {
                string fieldName = fieldNames[index];
                if (mapper(fieldName, out string mappedFieldName))
                {
                    fieldNames[index] = mappedFieldName;
                }
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), fieldNames);
        }

        protected override FieldPropertyMapperCollection<GroupField> PropertyMappers { get; } = new FieldPropertyMapperCollection<GroupField>
        {
            { f => f.ChildFieldNames, ColumnNames.LIST },
            { f => f.BackgroundColor, TryGetBackgroundColor }
        };
    }

    public class ImageFieldMapper : FieldMapper<ImageField>
    {
        protected override FieldPropertyMapperCollection<ImageField> PropertyMappers { get; } = new FieldPropertyMapperCollection<ImageField>
        {
            { f => f.ShouldRetainImageSize }
        };
    }

    public class InputFieldWithoutSeparatePromptMapper : FieldMapper<InputFieldWithoutSeparatePrompt>
    {
        protected override FieldPropertyMapperCollection<InputFieldWithoutSeparatePrompt> PropertyMappers { get; } = new FieldPropertyMapperCollection<InputFieldWithoutSeparatePrompt>
        {
            { f => f.ShouldRepeatLast },
            { f => f.IsRequired },
            { f => f.IsReadOnly }
        };
    }

    public class InputFieldWithSeparatePromptMapper : FieldMapper<InputFieldWithSeparatePrompt>
    {
        protected override FieldPropertyMapperCollection<InputFieldWithSeparatePrompt> PropertyMappers { get; } = new FieldPropertyMapperCollection<InputFieldWithSeparatePrompt>
        {
            { f => f.ShouldRepeatLast },
            { f => f.IsRequired },
            { f => f.IsReadOnly }
        };
    }

    public class MirrorFieldMapper : FieldMapper<MirrorField>
    {
        protected override FieldPropertyMapperCollection<MirrorField> PropertyMappers { get; } = new FieldPropertyMapperCollection<MirrorField>
        {
            { f => f.SourceFieldId }
        };
    }

    public class NumberFieldMapper : FieldMapper<NumberField>
    {
        protected override FieldPropertyMapperCollection<NumberField> PropertyMappers { get; } = new FieldPropertyMapperCollection<NumberField>
        {
            { f => f.Pattern },
            { f => f.Lower },
            { f => f.Upper }
        };
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

        protected override FieldPropertyMapperCollection<OptionField> PropertyMappers { get; } = new FieldPropertyMapperCollection<OptionField>
        {
            { f => f.Pattern },
            { f => f.ShowTextOnRight },
            { f => f.Options, TryGetOptions }
        };
    }

    public class PhoneNumberFieldMapper : FieldMapper<PhoneNumberField>
    {
        protected override FieldPropertyMapperCollection<PhoneNumberField> PropertyMappers { get; } = new FieldPropertyMapperCollection<PhoneNumberField>
        {
            { f => f.Pattern }
        };
    }

    public class RelatedViewFieldMapper : FieldMapper<RelatedViewField>
    {
        protected override FieldPropertyMapperCollection<RelatedViewField> PropertyMappers { get; } = new FieldPropertyMapperCollection<RelatedViewField>
        {
            { f => f.RelatedViewID, nameof(XField.RelatedViewId) },
            { f => f.ShouldReturnToParent },
            { f => f.Condition, ColumnNames.RELATE_CONDITION }
        };
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

        protected override FieldPropertyMapperCollection<RenderableField> PropertyMappers { get; } = new FieldPropertyMapperCollection<RenderableField>
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

    public class TableBasedDropDownFieldMapper : FieldMapper<TableBasedDropDownField>
    {
        protected override FieldPropertyMapperCollection<TableBasedDropDownField> PropertyMappers { get; } = new FieldPropertyMapperCollection<TableBasedDropDownField>
        {
            { f => f.ShouldSort, ColumnNames.SORT },
            { f => f.TextColumnName },
            { f => f.CodeColumnName },
            { f => f.SourceTableName },
            { f => f.IsExclusiveTable }
        };
    }

    public class TextFieldMapper : FieldMapper<TextField>
    {
        protected override FieldPropertyMapperCollection<TextField> PropertyMappers { get; } = new FieldPropertyMapperCollection<TextField>
        {
            { f => f.MaxLength },
            { f => f.SourceFieldId },
            { f => f.IsEncrypted }
        };
    }
}
