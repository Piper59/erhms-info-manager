using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : ViewModelBase
    {
        public class ResponderListViewModel : ListViewModelBase<Responder>
        {
            public RosterListViewModel Parent { get; private set; }
            public bool Rostered { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListViewModel(RosterListViewModel parent, bool rostered)
            {
                Parent = parent;
                Rostered = rostered;
                Selecting += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
            {
                return RosterListViewModel.GetFilteredValues(item);
            }

            protected override ICollectionView GetItems()
            {
                ICollection<string> responderIds = Parent.Rosters.Select(roster => roster.ResponderId).ToList();
                return CollectionViewSource.GetDefaultView(Parent.Responders
                    .Where(responder => responderIds.Contains(responder.ResponderId, StringComparer.OrdinalIgnoreCase) == Rostered)
                    .OrderBy(responder => responder.LastName)
                    .ThenBy(responder => responder.FirstName));
            }

            public void Edit()
            {
                Locator.Main.OpenResponderDetailView(SelectedItem);
            }
        }

        private static IEnumerable<string> GetFilteredValues(Responder item)
        {
            yield return item.ResponderId;
            yield return item.LastName;
            yield return item.FirstName;
            yield return item.EmailAddress;
            yield return item.City;
            yield return item.State;
            yield return item.OrganizationName;
            yield return item.Occupation;
        }


        public Incident Incident { get; private set; }
        private ICollection<Responder> Responders { get; set; }
        private ICollection<Roster> Rosters { get; set; }
        public ResponderListViewModel UnrosteredResponders { get; private set; }
        public ResponderListViewModel RosteredResponders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public RosterListViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            UnrosteredResponders = new ResponderListViewModel(this, false);
            RosteredResponders = new ResponderListViewModel(this, true);
            UnrosteredResponders.Selecting += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            RosteredResponders.Selecting += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            AddCommand = new RelayCommand(Add, HasSelectedUnrosteredResponder);
            RemoveCommand = new RelayCommand(Remove, HasSelectedRosteredResponder);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
        }

        private void UpdateTitle()
        {
            string incidentName = Incident.New ? "New Incident" : Incident.Name;
            Title = string.Format("{0} Roster", incidentName).Trim();
        }

        public bool HasSelectedUnrosteredResponder()
        {
            return UnrosteredResponders.HasSelectedItem();
        }

        public bool HasSelectedRosteredResponder()
        {
            return RosteredResponders.HasSelectedItem();
        }

        public void Add()
        {
            foreach (Responder responder in UnrosteredResponders.SelectedItems)
            {
                Roster roster = DataContext.Rosters.Create();
                roster.ResponderId = responder.ResponderId;
                roster.IncidentId = Incident.IncidentId;
                DataContext.Rosters.Save(roster);
            }
            Refresh();
        }

        public void Remove()
        {
            foreach (Responder responder in RosteredResponders.SelectedItems)
            {
                DataParameterCollection parameters = new DataParameterCollection(DataContext.Driver);
                parameters.AddByValue(responder.ResponderId);
                parameters.AddByValue(Incident.IncidentId);
                string sql = parameters.Format("ResponderId = {0} AND IncidentId = {1}");
                Roster roster = DataContext.Rosters.Select(new DataPredicate(sql, parameters)).FirstOrDefault();
                if (roster != null)
                {
                    DataContext.Rosters.Delete(roster);
                }
            }
            Refresh();
        }

        public void Refresh()
        {
            Responders = DataContext.Responders.SelectByDeleted(false).ToList();
            Rosters = DataContext.Rosters.SelectByIncident(Incident.IncidentId).ToList();
            UnrosteredResponders.Refresh();
            RosteredResponders.Refresh();
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
        {
            Refresh();
        }
    }
}
