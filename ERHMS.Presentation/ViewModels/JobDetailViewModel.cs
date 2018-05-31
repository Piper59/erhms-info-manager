using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class JobDetailViewModel : DocumentViewModel
    {
        public Job Job { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public JobDetailViewModel(Job job)
        {
            Title = job.New ? "New Job" : job.Name;
            Job = job;
            AddDirtyCheck(job);
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Job.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                await ServiceLocator.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Job.StartDate, Job.EndDate))
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.JobEndDateInvalid);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            Context.Jobs.Save(Job);
            ServiceLocator.Dialog.Notify(Resources.JobSaved);
            ServiceLocator.Data.Refresh(typeof(Job));
            Title = Job.Name;
            Dirty = false;
        }
    }
}
