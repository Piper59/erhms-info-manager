using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasViewModel : AnalysisViewModel
    {
        public CanvasViewModel(View view)
            : base(view)
        {
            Title = "Create a Dashboard";
        }

        public CanvasViewModel(int viewId)
            : this(Context.Views.SelectById(viewId)) { }

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
            ServiceLocator.Data.Refresh(typeof(Domain.Canvas));
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Wrapper wrapper = AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
