using ERHMS.Domain;
using ERHMS.Presentation.Messages;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmLinkViewModel : LinkViewModel<Pgm, PgmLink>
    {
        public PgmLinkViewModel(IServiceManager services, Pgm pgm)
            : base(services, pgm) { }

        public override void Link()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            Context.PgmLinks.Save(new PgmLink
            {
                PgmId = Entity.PgmId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Pgm)));
            Close();
        }

        public override void Unlink()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            MessengerInstance.Send(new RefreshMessage(typeof(Pgm)));
            Close();
        }
    }
}
