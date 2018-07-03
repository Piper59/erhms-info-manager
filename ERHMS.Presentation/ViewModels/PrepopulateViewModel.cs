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
            Title = "Prepopulate Responder Data";
            View = view;
            Responders = new ResponderListChildViewModel(view.Incident);
            ContinueCommand = new AsyncCommand(ContinueAsync);
        }

        public async Task ContinueAsync()
        {
            Close();
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            IDictionary<string, string> record = null;
            if (Responders.SelectedItem != null)
            {
                record = Context.Responders.Refresh(Responders.SelectedItem).ToRelatedRecord();
            }
            Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name, record);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
