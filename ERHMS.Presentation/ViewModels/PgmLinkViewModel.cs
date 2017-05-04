using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmLinkViewModel : LinkViewModelBase
    {
        public DeepLink<Pgm> PgmDeepLink { get; private set; }

        public PgmLinkViewModel(DeepLink<Pgm> pgmDeepLink)
            : base(pgmDeepLink.Incident?.IncidentId)
        {
            PgmDeepLink = pgmDeepLink;
        }

        public override void Link()
        {
            DataContext.PgmLinks.DeleteByPgmId(PgmDeepLink.Item.PgmId);
            PgmLink pgmLink = DataContext.PgmLinks.Create();
            pgmLink.PgmId = PgmDeepLink.Item.PgmId;
            pgmLink.IncidentId = Incidents.SelectedItem.IncidentId;
            DataContext.PgmLinks.Save(pgmLink);
            Messenger.Default.Send(new RefreshMessage<Pgm>());
            Active = false;
        }

        public override void Unlink()
        {
            DataContext.PgmLinks.DeleteByPgmId(PgmDeepLink.Item.PgmId);
            Messenger.Default.Send(new RefreshMessage<Pgm>());
            Active = false;
        }
    }
}
