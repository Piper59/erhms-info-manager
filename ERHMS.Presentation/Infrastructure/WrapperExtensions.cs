﻿using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using ERHMS.Presentation.ViewModels;

namespace ERHMS.Presentation
{
    public static class WrapperExtensions
    {
        public static void AddRecordSavedHandler(this Wrapper @this, IServiceManager services)
        {
            @this.Event += (sender, e) =>
            {
                if (e.Type == "RecordSaved" && e.Properties.ViewId == services.Data.Context.Responders.View.Id)
                {
                    services.Dispatch.Post(() =>
                    {
                        services.Data.Refresh(typeof(Responder));
                    });
                }
            };
        }

        public static void AddViewCreatedHandler(this Wrapper @this, IServiceManager services, Incident incident)
        {
            @this.Event += (sender, e) =>
            {
                if (e.Type == "ViewCreated")
                {
                    if (incident != null)
                    {
                        services.Data.Context.ViewLinks.Save(new ViewLink(true)
                        {
                            ViewId = e.Properties.ViewId,
                            IncidentId = incident.IncidentId
                        });
                    }
                    services.Dispatch.Post(() =>
                    {
                        services.Data.Refresh(typeof(View));
                        if (incident == null)
                        {
                            ViewListViewModel model = services.Document.ShowByType(() => new ViewListViewModel(services, null));
                            model.Views.SelectById(e.Properties.ViewId);
                        }
                        else
                        {
                            IncidentViewModel parent = services.Document.Show(
                                model => model.Incident.Equals(incident),
                                () => new IncidentViewModel(services, incident));
                            parent.Views.Active = true;
                            parent.Views.Views.SelectById(e.Properties.ViewId);
                        }
                    });
                }
            };
        }
    }
}
