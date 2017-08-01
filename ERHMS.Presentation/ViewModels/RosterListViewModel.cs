using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : ListViewModel<Roster>
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Incident Incident { get; private set; }

            private RelayCommand editCommand;
            public ICommand EditCommand
            {
                get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSelectedItem)); }
            }

            public ResponderListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                SelectionChanged += (sender, e) =>
                {
                    editCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectUnrostered(Incident.IncidentId).OrderBy(responder => responder.FullName);
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

        private RelayCommand addCommand;
        public ICommand AddCommand
        {
            get { return addCommand ?? (addCommand = new RelayCommand(Add, Responders.HasSelectedItem)); }
        }

        private RelayCommand removeCommand;
        public ICommand RemoveCommand
        {
            get { return removeCommand ?? (removeCommand = new RelayCommand(Remove, HasSelectedItem)); }
        }

        private RelayCommand editCommand;
        public ICommand EditCommand
        {
            get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSelectedItem)); }
        }

        private RelayCommand emailCommand;
        public ICommand EmailCommand
        {
            get { return emailCommand ?? (emailCommand = new RelayCommand(Email, HasSelectedItem)); }
        }

        public RosterListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Incident = incident;
            Responders = new ResponderListChildViewModel(services, incident);
            Responders.SelectionChanged += (sender, e) =>
            {
                addCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                removeCommand.RaiseCanExecuteChanged();
                editCommand.RaiseCanExecuteChanged();
                emailCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
                Context.Rosters.Save(new Roster
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
