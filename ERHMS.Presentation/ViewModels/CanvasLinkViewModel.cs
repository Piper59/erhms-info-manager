using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasLinkViewModel : LinkViewModelBase
    {
        public DeepLink<Canvas> CanvasDeepLink { get; private set; }

        public CanvasLinkViewModel(DeepLink<Canvas> canvasDeepLink)
            : base(canvasDeepLink.Incident?.IncidentId)
        {
            CanvasDeepLink = canvasDeepLink;
        }

        public override void Link()
        {
            DataContext.CanvasLinks.DeleteByCanvasId(CanvasDeepLink.Item.CanvasId);
            CanvasLink canvasLink = DataContext.CanvasLinks.Create();
            canvasLink.CanvasId = CanvasDeepLink.Item.CanvasId;
            canvasLink.IncidentId = Incidents.SelectedItem.IncidentId;
            DataContext.CanvasLinks.Save(canvasLink);
            Messenger.Default.Send(new RefreshMessage<Canvas>());
            Active = false;
        }

        public override void Unlink()
        {
            DataContext.CanvasLinks.DeleteByCanvasId(CanvasDeepLink.Item.CanvasId);
            Messenger.Default.Send(new RefreshMessage<Canvas>());
            Active = false;
        }
    }
}
