using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class PrepopulateViewModel : DialogViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Incident Incident { get; private set; }

            public ResponderListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                IEnumerable<Responder> responders = Incident == null
                    ? Context.Responders.SelectUndeleted()
                    : Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId).Select(roster => roster.Responder);
                return responders.OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }
        }

        public View View { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public ICommand ContinueCommand { get; private set; }

        public PrepopulateViewModel(View view)
        {
            Title = "Prepopulate Responder Data Fields";
            View = view;
            Responders = new ResponderListChildViewModel(view.Incident);
            ContinueCommand = new AsyncCommand(ContinueAsync);
        }

        private object GetRecord()
        {
            if (Responders.SelectedItem == null)
            {
                return null;
            }
            else
            {
                Responder responder = Context.Responders.Refresh(Responders.SelectedItem);
                return new
                {
                    ResponderID = responder.ResponderId,
                    ResponderEmailAddress = responder.EmailAddress,
                    ResponderLastName = responder.LastName,
                    ResponderFirstName = responder.FirstName,
                    ResponderMiddleInitial = responder.MiddleInitial,
                    ResponderFullName = responder.FullName,
                    ResponderName = responder.FullName
                };
            }
        }

        public async Task ContinueAsync()
        {
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name, GetRecord());
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
