using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using ERHMS.Presentation.ViewModels;

namespace ERHMS.Presentation
{
    public static class WrapperExtensions
    {
        public static void AddRecordSavedHandler(this Wrapper @this)
        {
            @this.Event += (sender, e) =>
            {
                if (e.Type != "RecordSaved")
                {
                    return;
                }
                if (e.Properties.ViewId == ServiceLocator.Data.Context.Responders.View.Id)
                {
                    ServiceLocator.Dispatcher.Post(() =>
                    {
                        ServiceLocator.Data.Refresh(typeof(Responder));
                    });
                }
            };
        }

        public static void AddViewCreatedHandler(this Wrapper @this, Incident incident)
        {
            @this.Event += (sender, e) =>
            {
                if (e.Type != "ViewCreated")
                {
                    return;
                }
                if (incident != null)
                {
                    ServiceLocator.Data.Context.ViewLinks.Save(new ViewLink(true)
                    {
                        ViewId = e.Properties.ViewId,
                        IncidentId = incident.IncidentId
                    });
                }
                ServiceLocator.Dispatcher.Post(() =>
                {
                    ServiceLocator.Data.Refresh(typeof(View));
                    if (incident == null)
                    {
                        ViewListViewModel model = ServiceLocator.Document.ShowByType(() => new ViewListViewModel(null));
                        model.Views.SelectById(e.Properties.ViewId);
                    }
                    else
                    {
                        IncidentViewModel model = ServiceLocator.Document.Show(
                            _model => _model.Incident.Equals(incident),
                            () => new IncidentViewModel(incident));
                        model.Views.Active = true;
                        model.Views.Views.SelectById(e.Properties.ViewId);
                    }
                });
            };
        }
    }
}
