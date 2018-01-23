using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasViewModel : AnalysisViewModel
    {
        public CanvasViewModel(IServiceManager services, View view)
            : base(services, view)
        {
            Title = "Create a Dashboard";
        }

        public CanvasViewModel(IServiceManager services, int viewId)
            : this(services, services.Data.Context.Views.SelectById(viewId)) { }

        public override async Task CreateAsync()
        {
            EpiInfo.Canvas canvas = new EpiInfo.Canvas
            {
                Name = Name,
                Content = EpiInfo.Canvas.GetContentForView(Context.Project.FilePath, View.Name)
            };
            Context.Project.InsertCanvas(canvas);
            if (View.Incident != null)
            {
                Context.CanvasLinks.Save(new CanvasLink(true)
                {
                    CanvasId = canvas.CanvasId,
                    IncidentId = View.Incident.IncidentId
                });
            }
            Services.Data.Refresh(typeof(Domain.Canvas));
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            await Services.Wrapper.InvokeAsync(AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content));
        }
    }
}
