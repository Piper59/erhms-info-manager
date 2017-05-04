using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : ViewModelBase
    {
        public class UnrosteredListViewModel : ListViewModelBase<Responder>
        {
            public Incident Incident { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public UnrosteredListViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
                EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
                SelectedItemChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                Messenger.Default.Register<RefreshMessage<Responder>>(this, msg => Refresh());
                Messenger.Default.Register<RefreshMessage<Roster>>(this, msg => Refresh());
            }

            protected override IEnumerable<Responder> GetItems()
            {
                ICollection<string> responderIds = DataContext.Rosters.SelectByIncidentId(Incident.IncidentId)
                    .Select(roster => roster.ResponderId)
                    .ToList();
                return DataContext.Responders.SelectUndeleted()
                    .Where(item => !responderIds.ContainsIgnoreCase(item.ResponderId))
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.FirstName);
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
                Main.OpenResponderDetailView(SelectedItem);
            }
        }

        public class RosteredListViewModel : ListViewModelBase<RosterViewModel>
        {
            public Incident Incident { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public RosteredListViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
                EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
                SelectedItemChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                Messenger.Default.Register<RefreshMessage<Responder>>(this, msg => Refresh());
                Messenger.Default.Register<RefreshMessage<Roster>>(this, msg => Refresh());
            }

            protected override IEnumerable<RosterViewModel> GetItems()
            {
                ICollection<Roster> rosters = DataContext.Rosters.SelectByIncidentId(Incident.IncidentId).ToList();
                IDictionary<string, Responder> responders = new Dictionary<string, Responder>(StringComparer.OrdinalIgnoreCase);
                foreach (Responder responder in DataContext.Responders.SelectUndeleted())
                {
                    responders[responder.ResponderId] = responder;
                }
                foreach (Roster roster in rosters)
                {
                    yield return new RosterViewModel(roster, responders[roster.ResponderId]);
                }
            }

            protected override IEnumerable<string> GetFilteredValues(RosterViewModel item)
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
                Main.OpenResponderDetailView(SelectedItem.Responder);
            }
        }

        public Incident Incident { get; private set; }
        public UnrosteredListViewModel Responders { get; private set; }
        public RosteredListViewModel Rosters { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public RosterListViewModel(Incident incident)
        {
            Incident = incident;
            Responders = new UnrosteredListViewModel(incident);
            Rosters = new RosteredListViewModel(incident);
            AddCommand = new RelayCommand(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new RelayCommand(Remove, Rosters.HasAnySelectedItems);
            EmailCommand = new RelayCommand(Email, Rosters.HasAnySelectedItems);
            RefreshCommand = new RelayCommand(Refresh);
            Responders.SelectedItemChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            Rosters.SelectedItemChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
            };
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Roster roster = DataContext.Rosters.Create();
                roster.ResponderId = responder.ResponderId;
                roster.IncidentId = Incident.IncidentId;
                DataContext.Rosters.Save(roster);
            }
            Messenger.Default.Send(new RefreshMessage<Roster>());
        }

        public void Remove()
        {
            foreach (RosterViewModel roster in Rosters.SelectedItems)
            {
                DataContext.Rosters.Delete(roster.Roster);
            }
            Messenger.Default.Send(new RefreshMessage<Roster>());
        }

        public void Email()
        {
            Main.OpenEmailView(new EmailViewModel(Rosters.TypedSelectedItems.Select(roster => roster.Responder)));
        }

        public void Refresh()
        {
            Responders.Refresh();
            Rosters.Refresh();
        }
    }
}
