using Epi;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating
{
    public class ProjectTemplateCreator : TemplateCreator
    {
        public Project Project { get; }
        protected override string DisplayName => Project.DisplayName;

        public ProjectTemplateCreator(Project project)
            : base(project.Metadata)
        {
            Project = project;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.Project);
            xTemplate.Name = Project.Name;
            xTemplate.Description = Project.Description;
            XProject xProject = XProject.Create(Project);
            xTemplate.Add(xProject);
            foreach (View view in Project.Views)
            {
                CreateXView(xProject, view);
            }
            return xTemplate;
        }
    }
}
