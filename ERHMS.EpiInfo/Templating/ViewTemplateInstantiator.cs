using Epi;
using ERHMS.EpiInfo.Templating.Xml;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public class ViewTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.View;
        public Project Project { get; }
        public View View { get; private set; }

        public ViewTemplateInstantiator(XTemplate xTemplate, Project project)
            : base(xTemplate, project.Metadata)
        {
            Project = project;
        }

        protected override void InstantiateCore()
        {
            XView xView = XTemplate.XProject.XViews.Single();
            View = InstantiateView(Project, xView);
        }
    }
}
