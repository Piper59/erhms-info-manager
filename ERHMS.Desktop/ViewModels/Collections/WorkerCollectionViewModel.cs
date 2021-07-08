using Epi;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class WorkerCollectionViewModel : CollectionViewModel<Worker>
    {
        public static async Task<WorkerCollectionViewModel> CreateAsync(
            string firstName,
            string lastName,
            string emailAddress)
        {
            WorkerCollectionViewModel result = new WorkerCollectionViewModel(firstName, lastName, emailAddress);
            await result.InitializeAsync();
            return result;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    Items.Refresh();
                }
            }
        }

        public RecordStatusListCollectionView Statuses { get; } = new RecordStatusListCollectionView();

        private WorkerCollectionViewModel(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Items.Filter = IsMatch;
            Items.PageSize = 100;
            Items.SortDescriptions.Add(new SortDescription(nameof(Worker.Similarity), ListSortDirection.Descending));
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                List.Clear();
                string projectPath = Settings.Default.WorkerProjectPath;
                Project project = ProjectExtensions.Open(projectPath);
                View view = project.Views[CoreView.WorkerRosteringForm.Name];
                using (RecordRepository<Worker> repository = new RecordRepository<Worker>(view))
                {
                    foreach (Worker worker in repository.Select())
                    {
                        worker.SetSimilarity(FirstName, LastName, EmailAddress);
                        List.Add(worker);
                    }
                }
            });
            Items.Refresh();
        }

        private bool IsStatusMatch(Worker worker)
        {
            RecordStatus? status = Statuses.CurrentItem?.Value;
            return status == null || worker.RECSTATUS == status;
        }

        private bool IsSearchMatch(Worker worker)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            if (worker.FullName?.Search(searchText) == true)
            {
                return true;
            }
            if (worker.EmailAddress?.Search(searchText) == true)
            {
                return true;
            }
            return false;
        }

        private bool IsMatch(object item)
        {
            Worker worker = (Worker)item;
            return IsStatusMatch(worker) && IsSearchMatch(worker);
        }
    }
}
