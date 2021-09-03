using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public ICommand AddCommand { get; }
        public ICommand ChooseCommand { get; }
        public ICommand RefreshCommand { get; }

        private WorkerCollectionViewModel(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Items.Filter = IsMatch;
            Items.PageSize = 100;
            Items.SortDescriptions.Add(new SortDescription(nameof(Worker.Similarity), ListSortDirection.Descending));
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
            AddCommand = new SyncCommand(Add);
            ChooseCommand = new SyncCommand(Choose, HasCurrent);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public event EventHandler<RecordEventArgs<Worker>> Committed;
        private void OnCommitted(RecordEventArgs<Worker> e) => Committed?.Invoke(this, e);
        private void OnCommitted(Worker worker) => OnCommitted(new RecordEventArgs<Worker>(worker));

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                List.Clear();
                Project project = ProjectExtensions.Open(Settings.Default.WorkerProjectPath);
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

        public void Add()
        {
            WizardViewModel wizard = CreateWorkerViewModels.GetWizard(FirstName, LastName, EmailAddress);
            if (wizard.Run(out CreateWorkerViewModels.State state) != true)
            {
                return;
            }
            OnCommitted(state.Worker);
        }

        public void Choose()
        {
            OnCommitted(CurrentItem);
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_RefreshingWorkers;
            await progress.Run(InitializeAsync);
        }
    }
}
