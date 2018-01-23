using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentListViewModel : DocumentViewModel
    {
        public class IncidentListChildViewModel : ListViewModel<Incident>
        {
            public IncidentListChildViewModel(IServiceManager services)
                : base(services)
            {
                Refresh();
            }

            protected override IEnumerable<Incident> GetItems()
            {
                return Context.Incidents.SelectUndeleted()
                    .OrderByDescending(incident => incident.StartDate)
                    .ThenBy(incident => incident.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Incident item)
            {
                yield return item.Name;
                yield return item.Description;
                yield return EnumExtensions.ToDescription(item.Phase);
                if (item.StartDate.HasValue)
                {
                    yield return item.StartDate.Value.ToShortDateString();
                }
            }
        }

        public IncidentListChildViewModel Incidents { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public IncidentListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Incidents";
            Incidents = new IncidentListChildViewModel(Services);
            CreateCommand = new Command(Create);
            OpenCommand = new Command(Open, Incidents.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Incidents.HasSelectedItem);
        }

        public void Create()
        {
            Services.Document.Show(() => new IncidentViewModel(Services, new Incident(true)));
        }

        public void Open()
        {
            Services.Document.Show(
                model => model.Incident.Equals(Incidents.SelectedItem),
                () => new IncidentViewModel(Services, Context.Incidents.Refresh(Incidents.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected incident?", "Delete"))
            {
                Incidents.SelectedItem.Deleted = true;
                Context.Incidents.Save(Incidents.SelectedItem);
                Services.Data.Refresh(typeof(Incident));
            }
        }

        public override void Dispose()
        {
            Incidents.Dispose();
            base.Dispose();
        }
    }
}
