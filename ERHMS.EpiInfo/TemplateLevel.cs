using System;

namespace ERHMS.EpiInfo
{
    public enum TemplateLevel
    {
        Unknown,
        Project,
        View,
        Page,
        Field
    }

    public static class TemplateLevelExtensions
    {
        public static TemplateLevel Parse(string value)
        {
            if (value.Equals("Form", StringComparison.OrdinalIgnoreCase))
            {
                return TemplateLevel.View;
            }
            Enum.TryParse(value, true, out TemplateLevel result);
            return result;
        }
    }
}
