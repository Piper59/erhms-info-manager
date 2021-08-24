using Epi;
using ERHMS.EpiInfo.Templating.Xml;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Templating
{
    public class ProjectTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.Project;
        public Project Project { get; }

        public ProjectTemplateInstantiator(XTemplate xTemplate, Project project)
            : base(xTemplate, project.Metadata)
        {
            Project = project;
        }

        protected override void InstantiateCore()
        {
            IDictionary<XView, View> viewsByXView = new Dictionary<XView, View>();
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                viewsByXView[xView] = InstantiateViewCore(Project, xView);
            }
            foreach (KeyValuePair<XView, View> viewByXView in viewsByXView)
            {
                OnInstantiating(TemplateLevel.View, viewByXView.Key.Name);
                Context.View = viewByXView.Value;
                foreach (XPage xPage in viewByXView.Key.XPages)
                {
                    InstantiatePage(viewByXView.Value, xPage);
                }
            }
        }
    }
}
