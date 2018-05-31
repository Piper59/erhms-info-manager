using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
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

            public ViewListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<View> GetItems()
            {
                IEnumerable<View> views = Incident == null
                    ? Context.Views.SelectUndeleted()
                    : Context.Views.SelectByIncidentId(Incident.IncidentId);
                return views.OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(Roster);
                }
            }

            public Incident Incident { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Responder> GetItems()
            {
                IEnumerable<Responder> responders = Incident == null
                    ? Context.Responders.SelectUndeleted()
                    : Context.Rosters.SelectUndeletedByIncidentId(Incident.IncidentId).Select(roster => roster.Responder);
                return responders.OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
            {
                return ListViewModelExtensions.GetFilteredValues(item);
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class AssignmentListChildViewModel : ListViewModel<Assignment>
        {
            public Incident Incident { get; private set; }

            public AssignmentListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Assignment> GetItems()
            {
                IEnumerable<Assignment> assignments = Incident == null
                    ? Context.Assignments.SelectUndeleted()
                    : Context.Assignments.SelectUndeletedByIncidentId(Incident.IncidentId);
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

        public Incident Incident { get; private set; }
        public ViewListChildViewModel Views { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public AssignmentListChildViewModel Assignments { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public AssignmentListViewModel(Incident incident)
        {
            Title = "Assignments";
            Incident = incident;
            Views = new ViewListChildViewModel(incident);
            Responders = new ResponderListChildViewModel(incident);
            Assignments = new AssignmentListChildViewModel(incident);
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
            using (ServiceLocator.Busy.Begin())
            {
                foreach (Responder responder in Responders.SelectedItems)
                {
                    Context.Assignments.Save(new Assignment(true)
                    {
                        ViewId = Views.SelectedItem.ViewId,
                        ResponderId = responder.ResponderId
                    });
                }
            }
            ServiceLocator.Data.Refresh(typeof(Assignment));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.AssignmentConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (Assignment assignment in Assignments.SelectedItems)
                    {
                        Context.Assignments.Delete(assignment);
                    }
                }
                ServiceLocator.Data.Refresh(typeof(Assignment));
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
            ServiceLocator.Document.Show(() =>
            {
                IEnumerable<Responder> responders = Assignments.SelectedItems.Select(assignment => assignment.Responder);
                EmailViewModel model = new EmailViewModel(Context.Responders.Refresh(responders));
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
    }
}
