using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobListViewModel : ListViewModelBase<Job>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public JobListViewModel(Incident incident)
        {
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Job>>(this, msg => Refresh());
        }

        protected override IEnumerable<Job> GetItems()
        {
            return DataContext.Jobs.SelectByIncidentId(Incident.IncidentId).OrderBy(item => item.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Job item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Create()
        {
            Job job = DataContext.Jobs.Create();
            job.IncidentId = Incident.IncidentId;
            Main.OpenJobDetailView(job);
        }

        public void Edit()
        {
            Main.OpenJobDetailView((Job)SelectedItem.Clone());
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
                DataContext.Jobs.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Job>());
            };
            Messenger.Default.Send(msg);
        }
    }
}
