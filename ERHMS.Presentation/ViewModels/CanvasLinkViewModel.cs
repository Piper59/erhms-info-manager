using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasLinkViewModel : LinkViewModel<Canvas, CanvasLink>
    {
        public CanvasLinkViewModel(IServiceManager services, Canvas canvas)
            : base(services, canvas) { }

        public override void Link()
        {
            Context.CanvasLinks.DeleteByCanvasId(Entity.CanvasId);
            Context.CanvasLinks.Save(new CanvasLink(true)
            {
                CanvasId = Entity.CanvasId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            Services.Data.Refresh(typeof(Canvas));
            Close();
        }

        public override void Unlink()
        {
            Context.CanvasLinks.DeleteByCanvasId(Entity.CanvasId);
            Services.Data.Refresh(typeof(Canvas));
            Close();
        }
    }
}
