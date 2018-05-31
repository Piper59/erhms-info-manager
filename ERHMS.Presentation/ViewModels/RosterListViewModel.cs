using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : DocumentViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(Roster);
                }
            }

            public Incident Incident { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectRosterable(Incident.IncidentId)
                    .OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
            {
                return ListViewModelExtensions.GetFilteredValues(item);
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class RosterListChildViewModel : ListViewModel<Roster>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(Roster);
                }
            }

            public Incident Incident { get; private set; }

            public ICommand EditCommand { get; private set; }

            public RosterListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Roster> GetItems()
            {
                return Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId)
                    .OrderBy(roster => roster.Responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Roster item)
            {
                return ListViewModelExtensions.GetFilteredValues(item.Responder);
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem.Responder)));
            }
        }

        public Incident Incident { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public RosterListChildViewModel Rosters { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public RosterListViewModel(Incident incident)
        {
            Title = "Roster";
            Incident = incident;
            Responders = new ResponderListChildViewModel(incident);
            Rosters = new RosterListChildViewModel(incident);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, Rosters.HasAnySelectedItems);
            EmailCommand = new Command(Email, Rosters.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (ServiceLocator.Busy.Begin())
            {
                foreach (Responder responder in Responders.SelectedItems)
                {
                    Context.Rosters.Save(new Roster(true)
                    {
                        ResponderId = responder.ResponderId,
                        IncidentId = Incident.IncidentId
                    });
                }
            }
            ServiceLocator.Data.Refresh(typeof(Roster));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.RosterConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (Roster roster in Rosters.SelectedItems)
                    {
                        Context.Rosters.Delete(roster);
                    }
                }
                ServiceLocator.Data.Refresh(typeof(Roster));
            }
        }

        public void Email()
        {
            ServiceLocator.Document.Show(() =>
            {
                IEnumerable<Responder> responders = Rosters.SelectedItems.Select(roster => roster.Responder);
                return new EmailViewModel(Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            Responders.Refresh();
            Rosters.Refresh();
        }
    }
}
