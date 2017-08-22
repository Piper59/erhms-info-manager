using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasViewModel : AnalysisViewModel
    {
        public CanvasViewModel(IServiceManager services, View view)
            : base(services, view)
        {
            Title = "Create a Dashboard";
        }

        public override void Create()
        {
            EpiInfo.Canvas canvas = new EpiInfo.Canvas
            {
                Name = Name,
                Content = EpiInfo.Canvas.GetContentForView(Context.Project.FilePath, View.Name)
            };
            Context.Project.InsertCanvas(canvas);
            if (View.Incident != null)
            {
                CanvasLink canvasLink = new CanvasLink(true)
                {
                    CanvasId = canvas.CanvasId,
                    IncidentId = View.Incident.IncidentId
                };
                Context.CanvasLinks.Save(canvasLink);
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Domain.Canvas)));
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Dialogs.InvokeAsync(AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content));
            Close();
        }
    }
}
