using Epi;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating
{
    public class PageTemplateCreator : TemplateCreator
    {
        public Page Page { get; }
        protected override string DisplayName => Page.DisplayName;

        public PageTemplateCreator(Page page)
            : base(page.GetMetadata())
        {
            Page = page;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.Page);
            xTemplate.Name = Page.Name;
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            XView xView = new XView
            {
                CheckCode = Page.GetView().CheckCode
            };
            xProject.Add(xView);
            CreateXPage(xView, Page);
            xView.Unrelate();
            return xTemplate;
        }
    }
}
