using System;

namespace ERHMS.EpiInfo.Templating
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
            else if (Enum.TryParse(value, out TemplateLevel result))
            {
                return result;
            }
            else
            {
                return TemplateLevel.Unknown;
            }
        }
    }
}
