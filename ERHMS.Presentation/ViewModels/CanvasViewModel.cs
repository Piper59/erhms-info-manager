using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasViewModel : AnalysisViewModel
    {
        public CanvasViewModel(DeepLink<View> viewDeepLink)
            : base(viewDeepLink)
        {
            Title = "Create a Dashboard";
        }

        public override void Create()
        {
            Canvas canvas = new Canvas
            {
                Name = Name,
                Content = Canvas.GetContentForView(ViewDeepLink.Item)
            };
            DataContext.Project.InsertCanvas(canvas);
            if (ViewDeepLink.Incident != null)
            {
                CanvasLink canvasLink = DataContext.CanvasLinks.Create();
                canvasLink.CanvasId = canvas.CanvasId;
                canvasLink.IncidentId = ViewDeepLink.Incident.IncidentId;
                DataContext.CanvasLinks.Save(canvasLink);
            }
            Messenger.Default.Send(new RefreshMessage<Canvas>());
            DataContext.Project.CollectedData.EnsureDataTablesExist(ViewDeepLink.Item);
            Wrapper wrapper = AnalysisDashboard.OpenCanvas.Create(DataContext.Project.FilePath, canvas.Content);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.CanvasSaved)
                {
                    canvas.Content = e.Properties.Content;
                    DataContext.Project.UpdateCanvas(canvas);
                    Messenger.Default.Send(new RefreshMessage<Canvas>());
                };
            };
            wrapper.Invoke();
            Active = false;
        }
    }
}
