using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
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
            public IncidentListChildViewModel()
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

        public IncidentListViewModel()
        {
            Title = "Incidents";
            Incidents = new IncidentListChildViewModel();
            CreateCommand = new Command(Create);
            OpenCommand = new Command(Open, Incidents.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Incidents.HasSelectedItem);
        }

        public void Create()
        {
            ServiceLocator.Document.Show(() => new IncidentViewModel(new Incident(true)));
        }

        public void Open()
        {
            ServiceLocator.Document.Show(
                model => model.Incident.Equals(Incidents.SelectedItem),
                () => new IncidentViewModel(Context.Incidents.Refresh(Incidents.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.IncidentConfirmDelete, "Delete"))
            {
                Incidents.SelectedItem.Deleted = true;
                Context.Incidents.Save(Incidents.SelectedItem);
                ServiceLocator.Data.Refresh(typeof(Incident));
            }
        }
    }
}
