using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobListViewModel : ListViewModel<Job>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        public JobListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Jobs";
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Job> GetItems()
        {
            return Context.Jobs.SelectByIncidentId(Incident.IncidentId)
                .WithResponders(Context)
                .OrderByDescending(job => job.StartDate)
                .ThenBy(job => job.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Job item)
        {
            yield return item.Name;
            yield return item.Description;
            if (item.StartDate.HasValue)
            {
                yield return item.StartDate.Value.ToShortDateString();
            }
            foreach (Responder responder in item.Responders)
            {
                yield return responder.FullName;
            }
        }

        public void Create()
        {
            Documents.ShowJob(new Job(true)
            {
                IncidentId = Incident.IncidentId
            });
        }

        public void Open()
        {
            Documents.ShowJob((Job)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected job?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Context.JobNotes.DeleteByJobId(SelectedItem.JobId);
                Context.JobTeams.DeleteByJobId(SelectedItem.JobId);
                Context.JobResponders.DeleteByJobId(SelectedItem.JobId);
                Context.JobLocations.DeleteByJobId(SelectedItem.JobId);
                Context.Jobs.Delete(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Job)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
