using System;

namespace ERHMS.EpiInfo.Templating
{
    public enum TemplateLevel
    {
        Project = 4,
        View = 3,
        Page = 2,
        Field = 1
    }

    public static class TemplateLevelExtensions
    {
        public static TemplateLevel Parse(string value)
        {
            if (value == "Form")
            {
                return TemplateLevel.View;
            }
            else
            {
                return (TemplateLevel)Enum.Parse(typeof(TemplateLevel), value);
            }
        }
    }
}
