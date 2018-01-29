using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
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
            public Incident Incident { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
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
                yield return item.LastName;
                yield return item.FirstName;
                yield return item.EmailAddress;
                yield return item.City;
                yield return item.State;
                yield return item.OrganizationName;
                yield return item.Occupation;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class RosterListChildViewModel : ListViewModel<Roster>
        {
            public Incident Incident { get; private set; }

            public ICommand EditCommand { get; private set; }

            public RosterListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
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
                yield return item.Responder.LastName;
                yield return item.Responder.FirstName;
                yield return item.Responder.EmailAddress;
                yield return item.Responder.City;
                yield return item.Responder.State;
                yield return item.Responder.OrganizationName;
                yield return item.Responder.Occupation;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem.Responder)));
            }
        }

        public Incident Incident { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public RosterListChildViewModel Rosters { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public RosterListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Roster";
            Incident = incident;
            Responders = new ResponderListChildViewModel(services, incident);
            Rosters = new RosterListChildViewModel(services, incident);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, Rosters.HasAnySelectedItems);
            EmailCommand = new Command(Email, Rosters.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (Services.Busy.BeginTask())
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
            Responders.Refresh();
            Services.Data.Refresh(typeof(Roster));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected responders?", "Remove"))
            {
                using (Services.Busy.BeginTask())
                {
                    foreach (Roster roster in Rosters.SelectedItems)
                    {
                        Context.Rosters.Delete(roster);
                    }
                }
                Responders.Refresh();
                Services.Data.Refresh(typeof(Roster));
            }
        }

        public void Email()
        {
            Services.Document.Show(() =>
            {
                IEnumerable<Responder> responders = Rosters.SelectedItems.Select(roster => roster.Responder);
                return new EmailViewModel(Services, Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            Responders.Refresh();
            Rosters.Refresh();
        }

        public override void Dispose()
        {
            Responders.Dispose();
            Rosters.Dispose();
            base.Dispose();
        }
    }
}
