using ERHMS.Domain;
using ERHMS.Presentation.Messages;

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
            MessengerInstance.Send(new RefreshMessage(typeof(Canvas)));
            Close();
        }

        public override void Unlink()
        {
            Context.CanvasLinks.DeleteByCanvasId(Entity.CanvasId);
            MessengerInstance.Send(new RefreshMessage(typeof(Canvas)));
            Close();
        }
    }
}
