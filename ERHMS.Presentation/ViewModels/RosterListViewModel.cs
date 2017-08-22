using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : ListViewModel<Roster>
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Incident Incident { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
                SelectionChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectRosterable(Incident.IncidentId).OrderBy(responder => responder.FullName);
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
                Documents.ShowResponder((Responder)SelectedItem.Clone());
            }
        }

        public Incident Incident { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public RosterListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Roster";
            Incident = incident;
            Responders = new ResponderListChildViewModel(services, incident);
            Refresh();
            AddCommand = new RelayCommand(Add, Responders.HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            Responders.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Roster> GetItems()
        {
            return Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId).OrderBy(roster => roster.Responder.FullName);
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

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.Rosters.Save(new Roster(true)
                {
                    ResponderId = responder.ResponderId,
                    IncidentId = Incident.IncidentId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Roster)));
        }

        public void Remove()
        {
            foreach (Roster roster in SelectedItems)
            {
                Context.Rosters.Delete(roster);
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Roster)));
        }

        public void Edit()
        {
            Documents.ShowResponder((Responder)SelectedItem.Responder.Clone());
        }

        public void Email()
        {
            Documents.Show(
                () => new EmailViewModel(Services, TypedSelectedItems.Select(roster => roster.Responder)),
                document => false);
        }

        public override void Refresh()
        {
            Responders.Refresh();
            base.Refresh();
        }
    }
}
