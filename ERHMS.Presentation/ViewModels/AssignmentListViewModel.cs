using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentListViewModel : ListViewModelBase<AssignmentViewModel>
    {
        public class ViewDeepLinkListViewModel : ListViewModelBase<DeepLink<View>>
        {
            public Incident Incident { get; private set; }

            public ViewDeepLinkListViewModel(Incident incident)
            {
                Incident = incident;
                Messenger.Default.Register<RefreshMessage<View>>(this, msg => Refresh());
            }

            protected override IEnumerable<DeepLink<View>> GetItems()
            {
                IEnumerable<DeepLink<View>> items;
                if (Incident == null)
                {
                    items = DataContext.ViewLinks.SelectDeepLinks();
                }
                else
                {
                    items = DataContext.ViewLinks.SelectDeepLinksByIncidentId(Incident.IncidentId);
                }
                return items.OrderBy(item => item.Item.Name);
            }
        }

        public class ResponderListViewModel : ListViewModelBase<Responder>
        {
            public Incident Incident { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListViewModel(Incident incident)
            {
                Incident = incident;
                EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
                SelectedItemChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                Messenger.Default.Register<RefreshMessage<Responder>>(this, msg => Refresh());
                if (incident != null)
                {
                    Messenger.Default.Register<RefreshMessage<Roster>>(this, msg => Refresh());
                }
            }

            protected override IEnumerable<Responder> GetItems()
            {
                IEnumerable<Responder> items;
                if (Incident == null)
                {
                    items = DataContext.Responders.SelectUndeleted();
                }
                else
                {
                    items = DataContext.Responders.SelectByIncidentId(Incident.IncidentId);
                }
                return items.OrderBy(item => item.LastName).ThenBy(item => item.FirstName);
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

        public Incident Incident { get; private set; }
        public ViewDeepLinkListViewModel ViewDeepLinks { get; private set; }
        public ResponderListViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public AssignmentListViewModel(Incident incident)
        {
            Title = "Assignments";
            Incident = incident;
            ViewDeepLinks = new ViewDeepLinkListViewModel(incident);
            Responders = new ResponderListViewModel(incident);
            Refresh();
            AddCommand = new RelayCommand(Add, CanAdd);
            RemoveCommand = new RelayCommand(Remove, HasAnySelectedItems);
            EmailCommand = new RelayCommand(Email, CanEmail);
            RefreshCommand = new RelayCommand(Refresh);
            ViewDeepLinks.SelectedItemChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            Responders.SelectedItemChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectedItemChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<View>>(this, msg => Refresh());
            Messenger.Default.Register<RefreshMessage<Responder>>(this, msg => Refresh());
            Messenger.Default.Register<RefreshMessage<Assignment>>(this, msg => Refresh());
        }

        public bool CanAdd()
        {
            return ViewDeepLinks.HasOneSelectedItem() && Responders.HasAnySelectedItems();
        }

        public bool CanEmail()
        {
            if (!HasAnySelectedItems())
            {
                return false;
            }
            else
            {
                int viewId = SelectedItem.Assignment.ViewId;
                return TypedSelectedItems.All(item => item.Assignment.ViewId == viewId);
            }
        }

        protected override IEnumerable<AssignmentViewModel> GetItems()
        {
            ICollection<Responder> responders = DataContext.Responders.SelectUndeleted().ToList();
            ICollection<AssignmentViewModel> items = new List<AssignmentViewModel>();
            foreach (Assignment assignment in DataContext.Assignments.Select())
            {
                DeepLink<View> itemViewDeepLink = ViewDeepLinks.TypedItems.SingleOrDefault(viewDeepLink => viewDeepLink.Item.Id == assignment.ViewId);
                Responder itemResponder = responders.SingleOrDefault(responder => responder.ResponderId.EqualsIgnoreCase(assignment.ResponderId));
                if (itemViewDeepLink != null && itemResponder != null)
                {
                    items.Add(new AssignmentViewModel(assignment, itemViewDeepLink, itemResponder));
                }
            }
            return items.OrderBy(item => item.ViewDeepLink.Item.Name).ThenBy(item => item.Responder.FullName);
        }

        protected override IEnumerable<string> GetFilteredValues(AssignmentViewModel item)
        {
            yield return item.ViewDeepLink.Item.Name;
            yield return item.Responder.FullName;
            if (Incident == null)
            {
                yield return item.ViewDeepLink.Incident?.Name;
            }
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Assignment assignment = DataContext.Assignments.Create();
                assignment.ViewId = ViewDeepLinks.SelectedItem.Item.Id;
                assignment.ResponderId = responder.ResponderId;
                DataContext.Assignments.Save(assignment);
            }
            Messenger.Default.Send(new RefreshMessage<Assignment>());
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected assignments?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (AssignmentViewModel item in SelectedItems)
                {
                    DataContext.Assignments.Delete(item.Assignment);
                }
                Messenger.Default.Send(new RefreshMessage<Assignment>());
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            EmailViewModel email = new EmailViewModel(TypedSelectedItems.Select(item => item.Responder));
            if (SelectedItem.ViewDeepLink.Item.IsWebSurvey())
            {
                email.AppendWebSurveyUrl = true;
                email.Views.SelectItem(view => view.Id == SelectedItem.ViewDeepLink.Item.Id);
                if (DataContext.IsResponderLinkedView(SelectedItem.ViewDeepLink.Item))
                {
                    email.PrepopulateResponderId = true;
                }
            }
            Main.OpenEmailView(email);
        }

        public override void Refresh()
        {
            ViewDeepLinks.Refresh();
            Responders.Refresh();
            base.Refresh();
        }
    }
}
