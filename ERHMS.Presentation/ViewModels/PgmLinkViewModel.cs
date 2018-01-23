using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmLinkViewModel : LinkViewModel<Pgm, PgmLink>
    {
        public PgmLinkViewModel(IServiceManager services, Pgm pgm)
            : base(services, pgm) { }

        public override void Link()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            Context.PgmLinks.Save(new PgmLink(true)
            {
                PgmId = Entity.PgmId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            Services.Data.Refresh(typeof(Pgm));
            Close();
        }

        public override void Unlink()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            Services.Data.Refresh(typeof(Pgm));
            Close();
        }
    }
}
