using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class OptionFieldMapper : FieldMapper<OptionField>
    {
        private static bool TryGetOptions(XField xField, out List<string> value)
        {
            if (xField.List == null)
            {
                value = default;
                return false;
            }
            else
            {
                string options = xField.List;
                int index = options.IndexOf("||");
                if (index != -1)
                {
                    options = options.Substring(0, index);
                }
                value = options.Split(Constants.LIST_SEPARATOR).ToList();
                return true;
            }
        }

        protected override MetaFieldType? FieldType => MetaFieldType.Option;
        protected override FieldPropertySetterCollection<OptionField> Setters { get; } =
            new FieldPropertySetterCollection<OptionField>
            {
                { field => field.Pattern },
                { field => field.ShowTextOnRight },
                { field => field.Options, TryGetOptions }
            };
    }
}
