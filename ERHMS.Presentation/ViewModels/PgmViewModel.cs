using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmViewModel : AnalysisViewModel
    {
        public PgmViewModel(IServiceManager services, View view)
            : base(services, view)
        {
            Title = "Create an Analysis";
        }

        public PgmViewModel(IServiceManager services, int viewId)
            : this(services, services.Data.Context.Views.SelectById(viewId)) { }

        public override async Task CreateAsync()
        {
            EpiInfo.Pgm pgm = new EpiInfo.Pgm
            {
                Name = Name,
                Content = EpiInfo.Pgm.GetContentForView(Context.Project.FilePath, View.Name)
            };
            Context.Project.InsertPgm(pgm);
            if (View.Incident != null)
            {
                Context.PgmLinks.Save(new PgmLink(true)
                {
                    PgmId = pgm.PgmId,
                    IncidentId = View.Incident.IncidentId
                });
            }
            Services.Data.Refresh(typeof(Domain.Pgm));
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            await Services.Wrapper.InvokeAsync(Analysis.OpenPgm.Create(pgm.Content, true));
        }
    }
}
