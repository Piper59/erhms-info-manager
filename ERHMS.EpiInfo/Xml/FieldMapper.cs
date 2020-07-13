using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public interface ITypedFieldMapper
    {
        void TrySetProperties(XElement element, Field field);
    }

    public class TypedFieldMapper<TModel> : Mapper<TModel>, ITypedFieldMapper
        where TModel : Field
    {
        protected override string ElementName => ElementNames.Field;

        public override XElement GetElement(TModel model)
        {
            throw new NotSupportedException();
        }

        public void TrySetProperties(XElement element, Field field)
        {
            if (field is TModel model)
            {
                SetProperties(element, model);
            }
        }
    }

    public class RenderableFieldMapper : TypedFieldMapper<RenderableField>
    {
        public RenderableFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.PromptText),
                Mapping.FromExpr(f => f.ControlWidthPercentage),
                Mapping.FromExpr(f => f.ControlHeightPercentage),
                Mapping.FromExpr(f => f.ControlLeftPositionPercentage),
                Mapping.FromExpr(f => f.ControlTopPositionPercentage),
                Mapping.FromExpr(f => f.TabIndex),
                Mapping.FromExpr(f => f.HasTabStop)
            };
        }

        public override void SetProperties(XElement element, RenderableField model)
        {
            base.SetProperties(element, model);
            model.ControlFont = element.ToFont("Control");
            model.PromptFont = element.ToFont("Prompt");
        }
    }

    public class FieldWithSeparatePromptMapper : TypedFieldMapper<FieldWithSeparatePrompt>
    {
        public FieldWithSeparatePromptMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.PromptLeftPositionPercentage),
                Mapping.FromExpr(f => f.PromptTopPositionPercentage)
            };
        }
    }

    public class InputFieldWithoutSeparatePromptMapper : TypedFieldMapper<InputFieldWithoutSeparatePrompt>
    {
        public InputFieldWithoutSeparatePromptMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.ShouldRepeatLast),
                Mapping.FromExpr(f => f.IsRequired),
                Mapping.FromExpr(f => f.IsReadOnly)
            };
        }
    }

    public class InputFieldWithSeparatePromptMapper : TypedFieldMapper<InputFieldWithSeparatePrompt>
    {
        public InputFieldWithSeparatePromptMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.ShouldRepeatLast),
                Mapping.FromExpr(f => f.IsRequired),
                Mapping.FromExpr(f => f.IsReadOnly)
            };
        }
    }

    public class TextFieldMapper : TypedFieldMapper<TextField>
    {
        public TextFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.MaxLength),
                Mapping.FromExpr(f => f.SourceFieldId),
                Mapping.FromExpr(f => f.IsEncrypted)
            };
        }
    }

    public class NumberFieldMapper : TypedFieldMapper<NumberField>
    {
        public NumberFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.Pattern),
                Mapping.FromExpr(f => f.Lower),
                Mapping.FromExpr(f => f.Upper)
            };
        }
    }

    public class PhoneNumberFieldMapper : TypedFieldMapper<PhoneNumberField>
    {
        public PhoneNumberFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.Pattern)
            };
        }
    }

    public class DateFieldMapper : TypedFieldMapper<DateField>
    {
        public DateFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.Lower),
                Mapping.FromExpr(f => f.Upper)
            };
        }
    }

    public class OptionFieldMapper : TypedFieldMapper<OptionField>
    {
        public OptionFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.Pattern),
                Mapping.FromExpr(f => f.ShowTextOnRight)
            };
        }

        public override void SetProperties(XElement element, OptionField model)
        {
            base.SetProperties(element, model);
            model.Options = element.Attribute(ColumnNames.LIST).ToOptions().ToList();
        }
    }

    public class ImageFieldMapper : TypedFieldMapper<ImageField>
    {
        public ImageFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.ShouldRetainImageSize)
            };
        }
    }

    public class MirrorFieldMapper : TypedFieldMapper<MirrorField>
    {
        public MirrorFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.SourceFieldId)
            };
        }
    }

    public class TableBasedDropDownFieldMapper : TypedFieldMapper<TableBasedDropDownField>
    {
        public TableBasedDropDownFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.ShouldSort, attributeName: ColumnNames.SORT),
                Mapping.FromExpr(f => f.TextColumnName),
                Mapping.FromExpr(f => f.CodeColumnName),
                Mapping.FromExpr(f => f.SourceTableName),
                Mapping.FromExpr(f => f.IsExclusiveTable)
            };
        }
    }

    public class DDLFieldOfCodesMapper : TypedFieldMapper<DDLFieldOfCodes>
    {
        public DDLFieldOfCodesMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.AssociatedFieldInformation, attributeName: ColumnNames.RELATE_CONDITION)
            };
        }
    }

    public class RelatedViewFieldMapper : TypedFieldMapper<RelatedViewField>
    {
        public RelatedViewFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.RelatedViewID, attributeName: "RelatedViewId"),
                Mapping.FromExpr(f => f.ShouldReturnToParent),
                Mapping.FromExpr(f => f.Condition, attributeName: ColumnNames.RELATE_CONDITION)
            };
        }
    }

    public class GroupFieldMapper : TypedFieldMapper<GroupField>
    {
        public GroupFieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.ChildFieldNames, attributeName: ColumnNames.LIST)
            };
        }

        public override void SetProperties(XElement element, GroupField model)
        {
            base.SetProperties(element, model);
            model.BackgroundColor = element.Attribute(ColumnNames.BACKGROUND_COLOR).ToColor();
        }
    }

    public class FieldMapper : Mapper<Field>
    {
        private readonly ICollection<ITypedFieldMapper> TypedMappers = new List<ITypedFieldMapper>
        {
            new RenderableFieldMapper(),
            new FieldWithSeparatePromptMapper(),
            new InputFieldWithoutSeparatePromptMapper(),
            new InputFieldWithSeparatePromptMapper(),
            new TextFieldMapper(),
            new NumberFieldMapper(),
            new PhoneNumberFieldMapper(),
            new DateFieldMapper(),
            new OptionFieldMapper(),
            new ImageFieldMapper(),
            new MirrorFieldMapper(),
            new TableBasedDropDownFieldMapper(),
            new DDLFieldOfCodesMapper(),
            new RelatedViewFieldMapper(),
            new GroupFieldMapper()
        };

        protected override string ElementName => ElementNames.Field;

        public FieldMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(f => f.Name)
            };
        }

        public override XElement GetElement(Field model)
        {
            throw new NotSupportedException();
        }

        public override void SetProperties(XElement element, Field model)
        {
            base.SetProperties(element, model);
            foreach (ITypedFieldMapper typedMapper in TypedMappers)
            {
                typedMapper.TrySetProperties(element, model);
            }
        }
    }
}
