using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentListViewModel : DocumentViewModel
    {
        public class ViewListChildViewModel : ListViewModel<View>
        {
            public Incident Incident { get; private set; }

            public ViewListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                Refresh();
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
                return views.OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

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
                IEnumerable<Responder> responders;
                if (Incident == null)
                {
                    responders = Context.Responders.SelectUndeleted();
                }
                else
                {
                    responders = Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId).Select(roster => roster.Responder);
                }
                return responders.OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
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

        public class AssignmentListChildViewModel : ListViewModel<Assignment>
        {
            public Incident Incident { get; private set; }

            public AssignmentListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                Refresh();
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
                return assignments.OrderBy(assignment => assignment.View.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(assignment => assignment.Responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Assignment item)
            {
                yield return item.View.Name;
                yield return item.Responder.FullName;
                yield return item.View.Incident?.Name;
            }
        }

        public ViewListChildViewModel Views { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public AssignmentListChildViewModel Assignments { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public AssignmentListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Assignments";
            Views = new ViewListChildViewModel(services, incident);
            Responders = new ResponderListChildViewModel(services, incident);
            Assignments = new AssignmentListChildViewModel(services, incident);
            AddCommand = new Command(Add, CanAdd);
            RemoveCommand = new AsyncCommand(RemoveAsync, Assignments.HasAnySelectedItems);
            EmailCommand = new Command(Email, CanEmail);
            RefreshCommand = new Command(Refresh);
        }

        public bool CanAdd()
        {
            return Views.HasSelectedItem() && Responders.HasAnySelectedItems();
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.Assignments.Save(new Assignment(true)
                {
                    ViewId = Views.SelectedItem.ViewId,
                    ResponderId = responder.ResponderId
                });
            }
            Services.Data.Refresh(typeof(Assignment));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected assignments?", "Remove"))
            {
                foreach (Assignment assignment in Assignments.SelectedItems)
                {
                    Context.Assignments.Delete(assignment);
                }
                Services.Data.Refresh(typeof(Assignment));
            }
        }

        public bool CanEmail()
        {
            if (!Assignments.HasAnySelectedItems())
            {
                return false;
            }
            else
            {
                View view = Assignments.SelectedItems.First().View;
                return Assignments.SelectedItems.All(assignment => assignment.View.Equals(view));
            }
        }

        public void Email()
        {
            Services.Document.Show(() =>
            {
                IEnumerable<Responder> responders = Assignments.SelectedItems.Select(assignment => assignment.Responder);
                EmailViewModel model = new EmailViewModel(Services, Context.Responders.Refresh(responders));
                View view = Context.Views.Refresh(Assignments.SelectedItems.First().View);
                if (ViewExtensions.IsWebSurvey(view?.WebSurveyId))
                {
                    model.AppendUrl = true;
                    model.Views.Select(view);
                }
                return model;
            });
        }

        public void Refresh()
        {
            Views.Refresh();
            Responders.Refresh();
            Assignments.Refresh();
        }

        public override void Dispose()
        {
            Views.Dispose();
            Responders.Dispose();
            Assignments.Dispose();
            base.Dispose();
        }
    }
}
