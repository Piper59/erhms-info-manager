using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasLinkViewModel : LinkViewModel<Canvas, CanvasLink>
    {
        public CanvasLinkViewModel(Canvas canvas)
            : base(canvas) { }

        public override void Link()
        {
            Context.CanvasLinks.DeleteByCanvasId(Entity.CanvasId);
            Context.CanvasLinks.Save(new CanvasLink(true)
            {
                CanvasId = Entity.CanvasId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            ServiceLocator.Data.Refresh(typeof(Canvas));
            Close();
        }

        public override void Unlink()
        {
            Context.CanvasLinks.DeleteByCanvasId(Entity.CanvasId);
            ServiceLocator.Data.Refresh(typeof(Canvas));
            Close();
        }
    }
}
