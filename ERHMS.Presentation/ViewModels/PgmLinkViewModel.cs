using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmLinkViewModel : LinkViewModel<Pgm, PgmLink>
    {
        public PgmLinkViewModel(Pgm pgm)
            : base(pgm) { }

        public override void Link()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            Context.PgmLinks.Save(new PgmLink(true)
            {
                PgmId = Entity.PgmId,
                IncidentId = Incidents.SelectedItem.IncidentId
            });
            ServiceLocator.Data.Refresh(typeof(Pgm));
            Close();
        }

        public override void Unlink()
        {
            Context.PgmLinks.DeleteByPgmId(Entity.PgmId);
            ServiceLocator.Data.Refresh(typeof(Pgm));
            Close();
        }
    }
}
