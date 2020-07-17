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
            if (value == "Form")
            {
                return TemplateLevel.View;
            }
            if (Enum.TryParse(value, out TemplateLevel result))
            {
                return result;
            }
            return TemplateLevel.Unknown;
        }
    }
}
