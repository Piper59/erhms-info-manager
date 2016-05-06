using Epi;
using ERHMS.Domain;
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
    public class AssignmentListViewModel : ListViewModelBase<AssignmentViewModel>
    {
        public class ResponderListInternalViewModel : ListViewModelBase<Responder>
        {
            public AssignmentListViewModel Parent { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListInternalViewModel(AssignmentListViewModel parent)
            {
                Parent = parent;
                Selecting += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
            }

            protected override ICollectionView GetItems()
            {
                IEnumerable<Responder> items;
                if (Parent.Incident == null)
                {
                    items = DataContext.Responders.SelectByDeleted(false);
                }
                else
                {
                    ICollection<string> responderIds = DataContext.Rosters.SelectByIncident(Parent.IncidentId)
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

        private ICollection<View> views;
        public ICollection<View> Views
        {
            get { return views; }
            set { Set(() => Views, ref views, value); }
        }

        private View selectedView;
        public View SelectedView
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
            Responders = new ResponderListInternalViewModel(this);
            Refresh();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "SelectedView")
                {
                    AddCommand.RaiseCanExecuteChanged();
                }
            };
            Responders.Selecting += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            Selecting += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
            AddCommand = new RelayCommand(Add, CanAddAssignment);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
        }

        private void UpdateTitle()
        {
            if (Incident == null)
            {
                Title = "Assignments";
            }
            else
            {
                string incidentName = Incident.New ? "New Incident" : Incident.Name;
                Title = string.Format("{0} Assignments", incidentName).Trim();
            }
        }

        public bool CanAddAssignment()
        {
            return SelectedView != null && Responders.HasSelectedItem();
        }

        protected override ICollectionView GetItems()
        {
            ICollection<Responder> responders = DataContext.Responders.SelectByDeleted(false).ToList();
            ICollection<AssignmentViewModel> items = new List<AssignmentViewModel>();
            foreach (Assignment assignment in DataContext.Assignments.Select())
            {
                View view = Views.SingleOrDefault(_view => _view.Id == assignment.ViewId);
                if (view != null)
                {
                    Responder responder = responders.SingleOrDefault(_responder => _responder.ResponderId.Equals(assignment.ResponderId, StringComparison.OrdinalIgnoreCase));
                    if (responder != null)
                    {
                        items.Add(new AssignmentViewModel(assignment, view, responder));
                    }
                }
            }
            return CollectionViewSource.GetDefaultView(items
                .OrderBy(assignment => assignment.View.Name)
                .ThenBy(assignment => assignment.Responder.LastName)
                .ThenBy(assignment => assignment.Responder.FirstName));
        }

        protected override IEnumerable<string> GetFilteredValues(AssignmentViewModel item)
        {
            yield return item.Responder.LastName;
            yield return item.Responder.FirstName;
            yield return item.View.Name;
        }

        public override void Refresh()
        {
            if (Incident == null)
            {
                Views = DataContext.GetUnlinkedViews().ToList();
            }
            else
            {
                Views = DataContext.GetLinkedViews(Incident.IncidentId).ToList();
            }
            Responders.Refresh();
            base.Refresh();
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Assignment assignment = DataContext.Assignments.Create();
                assignment.ViewId = SelectedView.Id;
                assignment.ResponderId = responder.ResponderId;
                DataContext.Assignments.Save(assignment);
            }
            Refresh();
        }

        public void Remove()
        {
            foreach (AssignmentViewModel assignment in SelectedItems)
            {
                DataContext.Assignments.Delete(assignment.Assignment);
            }
            Refresh();
        }

        public void Email()
        {
            // TODO: Implement
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
