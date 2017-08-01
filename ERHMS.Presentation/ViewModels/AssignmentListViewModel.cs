using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentListViewModel : ListViewModel<Assignment>
    {
        public class ViewListChildViewModel : ListViewModel<View>
        {
            public Incident Incident { get; private set; }

            public ViewListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
            }

            protected override IEnumerable<View> GetItems()
            {
                IEnumerable<View> views;
                if (Incident == null)
                {
                    views = Context.Views.SelectUndeleted();
                }
                else
                {
                    views = Context.Views.SelectByIncidentId(Incident.IncidentId);
                }
                return views.OrderBy(view => view.Name);
            }
        }

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
                IEnumerable<Responder> responders;
                if (Incident == null)
                {
                    responders = Context.Responders.SelectUndeleted();
                }
                else
                {
                    responders = Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId).Select(roster => roster.Responder);
                }
                return responders.OrderBy(responder => responder.FullName);
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
                Documents.ShowResponder(SelectedItem);
            }
        }

        public Incident Incident { get; private set; }
        public ViewListChildViewModel Views { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public AssignmentListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Assignments";
            Incident = incident;
            Views = new ViewListChildViewModel(services, incident);
            Responders = new ResponderListChildViewModel(services, incident);
            Refresh();
            AddCommand = new RelayCommand(Add, CanAdd);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, CanEmail);
            Views.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            Responders.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        public bool CanAdd()
        {
            return Views.HasSelectedItem() && Responders.HasSelectedItem();
        }

        public bool CanEmail()
        {
            if (!HasSelectedItem())
            {
                return false;
            }
            else
            {
                int viewId = SelectedItem.ViewId;
                return TypedSelectedItems.All(assignment => assignment.ViewId == viewId);
            }
        }

        protected override IEnumerable<Assignment> GetItems()
        {
            IEnumerable<Assignment> assignments;
            if (Incident == null)
            {
                assignments = Context.Assignments.SelectUndeleted();
            }
            else
            {
                assignments = Context.Assignments.SelectUndeletedByIncidentId(Incident.IncidentId);
            }
            return assignments.OrderBy(assignment => assignment.View.Name).ThenBy(assignment => assignment.Responder.FullName);
        }

        protected override IEnumerable<string> GetFilteredValues(Assignment item)
        {
            yield return item.View.Name;
            yield return item.Responder.FullName;
            yield return item.View.Incident?.Name;
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.Assignments.Save(new Assignment
                {
                    ViewId = Views.SelectedItem.ViewId,
                    ResponderId = responder.ResponderId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Assignment)));
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
                foreach (Assignment assignment in SelectedItems)
                {
                    Context.Assignments.Delete(assignment);
                }
                MessengerInstance.Send(new RefreshMessage(typeof(Assignment)));
            };
            MessengerInstance.Send(msg);
        }

        public void Email()
        {
            Documents.Show(
                () =>
                {
                    EmailViewModel document = new EmailViewModel(Services, TypedSelectedItems.Select(assignment => assignment.Responder));
                    if (ViewExtensions.IsWebSurvey(SelectedItem.View.WebSurveyId))
                    {
                        document.AppendUrl = true;
                        document.Views.Select(SelectedItem.ViewId);
                        if (document.Views.SelectedItem.HasResponderIdField)
                        {
                            document.Prepopulate = true;
                        }
                    }
                    return document;
                },
                document => false);
        }

        public override void Refresh()
        {
            Views.Refresh();
            Responders.Refresh();
            base.Refresh();
        }
    }
}
