using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewLinkViewModel : LinkViewModel<View, ViewLink>
    {
        public ViewLinkViewModel(View view)
            : base(view) { }

        public override void Link()
        {
            Context.ViewLinks.DeleteByViewId(Entity.ViewId);
            Context.ViewLinks.Save(new ViewLink(true)
            {
                ViewId = Entity.ViewId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            ServiceLocator.Data.Refresh(typeof(View));
            Close();
        }

        public override void Unlink()
        {
            Context.ViewLinks.DeleteByViewId(Entity.ViewId);
            ServiceLocator.Data.Refresh(typeof(View));
            Close();
        }
    }
}
