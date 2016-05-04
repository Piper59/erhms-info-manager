using System;
using System.ComponentModel;

namespace ERHMS.EpiInfo
{
    public enum TemplateLevel
    {
        [Description("Project")]
        Project,

        [Description("Form")]
        View,

        [Description("Page")]
        Page,

        [Description("Field")]
        Field
    }

    public static class TemplateLevelExtensions
    {
        public static bool TryParse(string value, out TemplateLevel result)
        {
            if (Enum.TryParse(value, out result))
            {
                return true;
            }
            else if (value == "Form")
            {
                result = TemplateLevel.View;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ToDirectoryName(this TemplateLevel @this)
        {
            switch (@this)
            {
                case TemplateLevel.Project:
                    return "Projects";
                case TemplateLevel.View:
                    return "Forms";
                case TemplateLevel.Page:
                    return "Pages";
                case TemplateLevel.Field:
                    return "Fields";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
