using Epi;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Xml
{
    public class ProjectMapper : Mapper<Project>
    {
        private static readonly ICollection<string> ConfigurationMappings = new string[]
        {
            "ControlFontBold",
            "ControlFontItalics",
            "ControlFontName",
            "ControlFontSize",
            "DefaultLabelAlign",
            "DefaultPageHeight",
            "DefaultPageOrientation",
            "DefaultPageWidth",
            "EditorFontBold",
            "EditorFontItalics",
            "EditorFontName",
            "EditorFontSize"
        };

        static ProjectMapper()
        {
            ElementName = "Project";
        }

        public ProjectMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.Constant("", Mapping.Ignored, "Id"),
                Mapping.FromExpr(p => p.Name, Mapping.Ignored),
                Mapping.Constant("", Mapping.Ignored, "Location"),
                Mapping.FromExpr(p => p.Description, Mapping.Ignored),
                Mapping.FromExpr(p => p.EpiVersion, Mapping.Ignored),
                Mapping.Constant("", Mapping.Ignored, "CreateDate")
            };
            Configuration configuration = Configuration.GetNewInstance();
            foreach (string mapping in ConfigurationMappings)
            {
                Mappings.Add(Mapping.Constant(configuration.Settings[mapping], Mapping.Ignored, mapping));
            }
        }
    }
}
