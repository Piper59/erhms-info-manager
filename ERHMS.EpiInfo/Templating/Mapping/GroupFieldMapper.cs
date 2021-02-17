using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class GroupFieldMapper : FieldMapper<GroupField>
    {
        private static bool TryGetBackgroundColor(XField xField, out Color value)
        {
            if (xField.BackgroundColor == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = Color.FromArgb(xField.BackgroundColor.Value);
                return true;
            }
        }

        protected override MetaFieldType? FieldType => MetaFieldType.Group;
        protected override FieldPropertySetterCollection<GroupField> PropertySetters { get; } =
            new FieldPropertySetterCollection<GroupField>
            {
                { field => field.ChildFieldNames, nameof(XField.List) },
                { field => field.BackgroundColor, TryGetBackgroundColor }
            };

        private bool MapChildFieldNames(string value, out string result)
        {
            if (value == null)
            {
                result = default;
                return false;
            }
            bool changed = false;
            IList<string> fieldNames = value.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < fieldNames.Count; index++)
            {
                string fieldName = fieldNames[index];
                if (MappingContext.MapFieldName(fieldName, out fieldName))
                {
                    fieldNames[index] = fieldName;
                    changed = true;
                }
            }
            result = string.Join(Constants.LIST_SEPARATOR.ToString(), fieldNames);
            return changed;
        }

        public override bool MapProperties(GroupField field)
        {
            bool changed = false;
            if (MapChildFieldNames(field.ChildFieldNames, out string result))
            {
                field.ChildFieldNames = result;
                changed = true;
            }
            return changed;
        }

        public override bool MapProperties(XField xField)
        {
            bool changed = false;
            if (MapChildFieldNames(xField.List, out string result))
            {
                xField.List = result;
                changed = true;
            }
            return changed;
        }

        public override bool Canonize(XField xField)
        {
            bool changed = false;
            if (xField.BackgroundColor != null)
            {
                Color backgroundColor = Color.FromArgb(xField.BackgroundColor.Value);
                Color opaqueBackgroundColor = Color.FromArgb(0xff, backgroundColor);
                xField.BackgroundColor = opaqueBackgroundColor.ToArgb();
                changed = true;
            }
            if (xField.List != null)
            {
                IReadOnlyCollection<string> childFieldNames =
                    new HashSet<string>(xField.ListItems, NameComparer.Default);
                xField.ListItems = xField.XPage.XFields
                    .Select(pageXField => pageXField.Name)
                    .Where(fieldName => childFieldNames.Contains(fieldName));
                changed = true;
            }
            return changed;
        }
    }
}
