using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;

namespace ERHMS.Presentation.ViewModels
{
    public class JobDetailViewModel : ViewModelBase
    {
        public Job Job { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public JobDetailViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = job.New ? "New Job" : job.Name;
            Job = job;
            SaveCommand = new RelayCommand(Save);
            AddDirtyCheck(job);
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Job.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (!ValidateDateRange(Job.StartDate, Job.EndDate))
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "End date must be later than start date."
                });
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Context.Jobs.Save(Job);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Job has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Job)));
            Title = Job.Name;
            Dirty = false;
        }
    }
}
