using Epi;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating
{
    public class ViewTemplateCreator : TemplateCreator
    {
        public View View { get; }

        public ViewTemplateCreator(View view)
            : base(view.GetMetadata())
        {
            View = view;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.View);
            xTemplate.Name = View.Name;
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            XView xView = CreateXView(xProject, View);
            xView.Unrelate();
            return xTemplate;
        }
    }
}
