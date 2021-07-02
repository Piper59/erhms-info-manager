using Epi;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating
{
    public class PageTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.Page;
        public View View { get; }
        public Page Page { get; private set; }

        public PageTemplateInstantiator(XTemplate xTemplate, View view)
            : base(xTemplate, view.GetMetadata())
        {
            View = view;
        }

        protected override void InstantiateCore()
        {
            XView xView = XTemplate.XProject.XView;
            string checkCode = xView.CheckCode.Trim();
            if (!View.CheckCode.Contains(checkCode))
            {
                View.CheckCode = $"{View.CheckCode}\n{checkCode}".Trim();
                Metadata.UpdateView(View);
            }
            Context.View = View;
            Page = InstantiatePage(View, xView.XPage);
        }
    }
}
