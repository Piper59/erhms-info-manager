using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentListViewModel : ListViewModelBase<AssignmentViewModel>
    {
        public class ResponderListInternalViewModel : ListViewModelBase<Responder>
        {
            public string IncidentId { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListInternalViewModel(string incidentId)
            {
                IncidentId = incidentId;
                Selecting += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
            }

            protected override ICollectionView GetItems()
            {
                IEnumerable<Responder> items;
                if (IncidentId == null)
                {
                    items = DataContext.Responders.SelectByDeleted(false);
                }
                else
                {
                    ICollection<string> responderIds = DataContext.Rosters.SelectByIncident(IncidentId)
                        .Select(roster => roster.ResponderId)
                        .ToList();
                    items = DataContext.Responders.SelectByDeleted(false)
                        .Where(responder => responderIds.Contains(responder.ResponderId, StringComparer.OrdinalIgnoreCase));
                }
                return CollectionViewSource.GetDefaultView(items.OrderBy(responder => responder.LastName).ThenBy(responder => responder.FirstName));
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
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

            public void Edit()
            {
                Locator.Main.OpenResponderDetailView(SelectedItem);
            }
        }

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        private ICollection<Link<View>> views;
        public ICollection<Link<View>> Views
        {
            get { return views; }
            set { Set(() => Views, ref views, value); }
        }

        private Link<View> selectedView;
        public Link<View> SelectedView
        {
            get { return selectedView; }
            set { Set(() => SelectedView, ref selectedView, value); }
        }

        public ResponderListInternalViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public AssignmentListViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SelectedView))
                {
                    AddCommand.RaiseCanExecuteChanged();
                }
            };
            Responders = new ResponderListInternalViewModel(IncidentId);
            Responders.Selecting += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            Selecting += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
            AddCommand = new RelayCommand(Add, CanAddAssignment);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, CanEmail);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<View>>(this, OnRefreshViewListMessage);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
            Messenger.Default.Register<RefreshListMessage<Roster>>(this, OnRefreshRosterListMessage);
            Messenger.Default.Register<RefreshListMessage<Assignment>>(this, OnRefreshAssignmentListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitle("Assignments", Incident);
        }

        public bool CanAddAssignment()
        {
            return SelectedView != null && Responders.HasSelectedItem();
        }

        public bool CanEmail()
        {
            return HasSelectedItem() && SelectedItems.Cast<AssignmentViewModel>().All(assignment => assignment.View == SelectedItem.View);
        }

        protected override ICollectionView GetItems()
        {
            ICollection<Responder> responders = DataContext.Responders.SelectByDeleted(false).ToList();
            ICollection<AssignmentViewModel> items = new List<AssignmentViewModel>();
            foreach (Assignment assignment in DataContext.Assignments.Select())
            {
                Link<View> view = Views.SingleOrDefault(_view => _view.Data.Id == assignment.ViewId);
                Responder responder = responders.SingleOrDefault(_responder => _responder.ResponderId.EqualsIgnoreCase(assignment.ResponderId));
                if (view != null && (view.Incident == null || !view.Incident.Deleted) && responder != null)
                {
                    items.Add(new AssignmentViewModel(assignment, view, responder));
                }
            }
            return CollectionViewSource.GetDefaultView(items.OrderBy(assignment => assignment.View.Data.Name)
                .ThenBy(assignment => assignment.Responder.LastName)
                .ThenBy(assignment => assignment.Responder.FirstName));
        }

        protected override IEnumerable<string> GetFilteredValues(AssignmentViewModel item)
        {
            yield return item.View.Data.Name;
            yield return item.Responder.LastName;
            yield return item.Responder.FirstName;
            yield return item.IncidentName;
        }

        public override void Refresh()
        {
            if (Incident == null)
            {
                Views = DataContext.GetLinkedViews()
                    .Where(view => view.Incident == null || !view.Incident.Deleted)
                    .OrderBy(view => view.Data.Name)
                    .ToList();
            }
            else
            {
                Views = DataContext.GetLinkedViews(IncidentId)
                    .Select(view => new Link<View>(view, Incident))
                    .OrderBy(view => view.Data.Name)
                    .ToList();
            }
            Responders.Refresh();
            base.Refresh();
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Assignment assignment = DataContext.Assignments.Create();
                assignment.ViewId = SelectedView.Data.Id;
                assignment.ResponderId = responder.ResponderId;
                DataContext.Assignments.Save(assignment);
            }
            Messenger.Default.Send(new RefreshListMessage<Assignment>(SelectedView.IncidentId));
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage("Remove", "Remove the selected assignments?");
            msg.Confirmed += (sender, e) =>
            {
                foreach (AssignmentViewModel assignment in SelectedItems)
                {
                    DataContext.Assignments.Delete(assignment.Assignment);
                }
                Messenger.Default.Send(new RefreshListMessage<Assignment>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            EmailViewModel email = new EmailViewModel(SelectedItems.Cast<AssignmentViewModel>().Select(assignment => assignment.Responder));
            if (SelectedItem.View.Data.IsPublished())
            {
                email.AppendUrl = true;
                email.SetSelectedView(SelectedItem.View.Data.Name);
                if (DataContext.IsResponderLinkedView(SelectedItem.View.Data))
                {
                    email.Prepopulate = true;
                }
            }
            Locator.Main.OpenEmailView(email);
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshViewListMessage(RefreshListMessage<View> msg)
        {
            if (Incident == null || StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }

        private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
        {
            Refresh();
        }

        private void OnRefreshRosterListMessage(RefreshListMessage<Roster> msg)
        {
            if (StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }

        private void OnRefreshAssignmentListMessage(RefreshListMessage<Assignment> msg)
        {
            if (Incident == null || StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
