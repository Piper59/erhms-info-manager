using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class FieldMapping<TField>
    {
        public static FieldMapping<TField> Create<TProperty>(
            Expression<Func<TField, TProperty>> expression,
            string attributeName = null)
        {
            PropertyInfo property = (PropertyInfo)((MemberExpression)expression.Body).Member;
            return new FieldMapping<TField>(property);
        }

        private string attributeName;
        private PropertyInfo property;

        public FieldMapping(PropertyInfo property, string attributeName = null)
        {
            this.attributeName = attributeName ?? property.Name;
            this.property = property;
        }

        public void Map(XElement element, TField field)
        {
            try
            {
                XAttribute attribute = element.Attribute(attributeName);
                if (string.IsNullOrEmpty(attribute.Value))
                {
                    return;
                }
                object value = Convert.ChangeType(attribute.Value, property.PropertyType);
                property.SetValue(field, value);
            }
            catch { }
        }
    }

    public class FieldMappingCollection<TField> : List<FieldMapping<TField>>
    {
        public void Add<TProperty>(Expression<Func<TField, TProperty>> expression, string attributeName = null)
        {
            Add(FieldMapping<TField>.Create(expression, attributeName));
        }
    }

    public interface IFieldMapper
    {
        void Map(XElement element, Field field);
    }

    public class FieldMapper<TField> : IFieldMapper
        where TField : Field
    {
        public FieldMappingCollection<TField> Mappings { get; set; }

        public virtual void Map(XElement element, TField field)
        {
            foreach (FieldMapping<TField> mapping in Mappings)
            {
                mapping.Map(element, field);
            }
        }

        public void Map(XElement element, Field field)
        {
            if (field is TField typedField)
            {
                Map(element, typedField);
            }
        }
    }

    public class RenderableFieldMapper : FieldMapper<RenderableField>
    {
        private static Font GetFont(XElement element, string prefix)
        {
            try
            {
                string family = (string)element.Attribute($"{prefix}FontFamily");
                float size = (float)element.Attribute($"{prefix}FontSize");
                if (!Enum.TryParse((string)element.Attribute($"{prefix}FontStyle"), true, out FontStyle style))
                {
                    style = FontStyle.Regular;
                }
                return new Font(family, size, style);
            }
            catch
            {
                return null;
            }
        }

        public RenderableFieldMapper()
        {
            Mappings = new FieldMappingCollection<RenderableField>
            {
                f => f.PromptText,
                f => f.ControlWidthPercentage,
                f => f.ControlHeightPercentage,
                f => f.ControlLeftPositionPercentage,
                f => f.ControlTopPositionPercentage,
                f => f.TabIndex,
                f => f.HasTabStop
            };
        }

        public override void Map(XElement element, RenderableField field)
        {
            base.Map(element, field);
            field.ControlFont = GetFont(element, "Control");
            field.PromptFont = GetFont(element, "Prompt");
        }
    }

    public class FieldWithSeparatePromptMapper : FieldMapper<FieldWithSeparatePrompt>
    {
        public FieldWithSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<FieldWithSeparatePrompt>
            {
                f => f.PromptLeftPositionPercentage,
                f => f.PromptTopPositionPercentage
            };
        }
    }

    public class InputFieldWithoutSeparatePromptMapper : FieldMapper<InputFieldWithoutSeparatePrompt>
    {
        public InputFieldWithoutSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<InputFieldWithoutSeparatePrompt>
            {
                f => f.ShouldRepeatLast,
                f => f.IsRequired,
                f => f.IsReadOnly
            };
        }
    }

    public class InputFieldWithSeparatePromptMapper : FieldMapper<InputFieldWithSeparatePrompt>
    {
        public InputFieldWithSeparatePromptMapper()
        {
            Mappings = new FieldMappingCollection<InputFieldWithSeparatePrompt>
            {
                f => f.ShouldRepeatLast,
                f => f.IsRequired,
                f => f.IsReadOnly
            };
        }
    }

    public class TextFieldMapper : FieldMapper<TextField>
    {
        public TextFieldMapper()
        {
            Mappings = new FieldMappingCollection<TextField>
            {
                f => f.MaxLength,
                f => f.SourceFieldId,
                f => f.IsEncrypted
            };
        }
    }

    public class NumberFieldMapper : FieldMapper<NumberField>
    {
        public NumberFieldMapper()
        {
            Mappings = new FieldMappingCollection<NumberField>
            {
                f => f.Pattern,
                f => f.Lower,
                f => f.Upper
            };
        }
    }

    public class PhoneNumberFieldMapper : FieldMapper<PhoneNumberField>
    {
        public PhoneNumberFieldMapper()
        {
            Mappings = new FieldMappingCollection<PhoneNumberField>
            {
                f => f.Pattern
            };
        }
    }

    public class DateFieldMapper : FieldMapper<DateField>
    {
        public DateFieldMapper()
        {
            Mappings = new FieldMappingCollection<DateField>
            {
                f => f.Lower,
                f => f.Upper
            };
        }
    }

    public class OptionFieldMapper : FieldMapper<OptionField>
    {
        private const string OptionsSeparator = "||";

        private static IEnumerable<string> GetOptions(XElement element)
        {
            try
            {
                string list = (string)element.Attribute(ColumnNames.LIST);
                int index = list.IndexOf(OptionsSeparator);
                string options = index == -1 ? list : list.Substring(0, index);
                return options.Split(Constants.LIST_SEPARATOR);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public OptionFieldMapper()
        {
            Mappings = new FieldMappingCollection<OptionField>
            {
                f => f.Pattern,
                f => f.ShowTextOnRight
            };
        }

        public override void Map(XElement element, OptionField model)
        {
            base.Map(element, model);
            model.Options = GetOptions(element).ToList();
        }
    }

    public class ImageFieldMapper : FieldMapper<ImageField>
    {
        public ImageFieldMapper()
        {
            Mappings = new FieldMappingCollection<ImageField>
            {
                f => f.ShouldRetainImageSize
            };
        }
    }

    public class MirrorFieldMapper : FieldMapper<MirrorField>
    {
        public MirrorFieldMapper()
        {
            Mappings = new FieldMappingCollection<MirrorField>
            {
                f => f.SourceFieldId
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
                f => f.TextColumnName,
                f => f.CodeColumnName,
                f => f.SourceTableName,
                f => f.IsExclusiveTable
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
                f => f.ShouldReturnToParent,
                { f => f.Condition, ColumnNames.RELATE_CONDITION }
            };
        }
    }

    public class GroupFieldMapper : FieldMapper<GroupField>
    {
        private static Color GetBackgroundColor(XElement element)
        {
            try
            {
                return Color.FromArgb((int)element.Attribute(ColumnNames.BACKGROUND_COLOR));
            }
            catch
            {
                return Color.Empty;
            }
        }

        public GroupFieldMapper()
        {
            Mappings = new FieldMappingCollection<GroupField>
            {
                { f => f.ChildFieldNames, ColumnNames.LIST }
            };
        }

        public override void Map(XElement element, GroupField model)
        {
            base.Map(element, model);
            model.BackgroundColor = GetBackgroundColor(element);
        }
    }
}
