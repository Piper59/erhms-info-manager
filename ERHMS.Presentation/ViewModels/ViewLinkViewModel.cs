using Epi;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewLinkViewModel : LinkViewModelBase
    {
        public DeepLink<View> ViewDeepLink { get; private set; }

        public ViewLinkViewModel(DeepLink<View> viewDeepLink)
            : base(viewDeepLink.Incident?.IncidentId)
        {
            ViewDeepLink = viewDeepLink;
        }

        public override void Link()
        {
            DataContext.ViewLinks.DeleteByViewId(ViewDeepLink.Item.Id);
            ViewLink viewLink = DataContext.ViewLinks.Create();
            viewLink.ViewId = ViewDeepLink.Item.Id;
            viewLink.IncidentId = Incidents.SelectedItem.IncidentId;
            DataContext.ViewLinks.Save(viewLink);
            Messenger.Default.Send(new RefreshMessage<View>());
            Active = false;
        }

        public override void Unlink()
        {
            DataContext.ViewLinks.DeleteByViewId(ViewDeepLink.Item.Id);
            Messenger.Default.Send(new RefreshMessage<View>());
            Active = false;
        }
    }
}
