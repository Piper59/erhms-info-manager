using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmViewModel : AnalysisViewModel
    {
        public PgmViewModel(View view)
            : base(view)
        {
            Title = "Create an Analysis";
        }

        public PgmViewModel(int viewId)
            : this(Context.Views.SelectById(viewId)) { }

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
            ServiceLocator.Data.Refresh(typeof(Domain.Pgm));
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Wrapper wrapper = Analysis.OpenPgm.Create(pgm.Content, true);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
