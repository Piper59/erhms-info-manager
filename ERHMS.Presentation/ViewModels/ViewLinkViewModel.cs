using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewLinkViewModel : LinkViewModel<View, ViewLink>
    {
        public ViewLinkViewModel(IServiceManager services, View view)
            : base(services, view) { }

        public override void Link()
        {
            Context.ViewLinks.DeleteByViewId(Entity.ViewId);
            Context.ViewLinks.Save(new ViewLink(true)
            {
                ViewId = Entity.ViewId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            Services.Data.Refresh(typeof(View));
            Close();
        }

        public override void Unlink()
        {
            Context.ViewLinks.DeleteByViewId(Entity.ViewId);
            Services.Data.Refresh(typeof(View));
            Close();
        }
    }
}
