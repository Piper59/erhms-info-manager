using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmViewModel : AnalysisViewModel
    {
        public PgmViewModel(IServiceManager services, View view)
            : base(services, view)
        {
            Title = "Create an Analysis";
        }

        public override void Create()
        {
            EpiInfo.Pgm pgm = new EpiInfo.Pgm
            {
                Name = Name,
                Content = EpiInfo.Pgm.GetContentForView(Context.Project.FilePath, View.Name)
            };
            Context.Project.InsertPgm(pgm);
            if (View.Incident != null)
            {
                PgmLink pgmLink = new PgmLink
                {
                    PgmId = pgm.PgmId,
                    IncidentId = View.Incident.IncidentId
                };
                Context.PgmLinks.Save(pgmLink);
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Domain.Pgm)));
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Analysis.OpenPgm.Create(pgm.Content, true).Invoke();
            Close();
        }
    }
}
