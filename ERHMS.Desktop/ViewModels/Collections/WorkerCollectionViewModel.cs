using Epi;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class WorkerCollectionViewModel : CollectionViewModelBase<Worker>
    {
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

        public RecordStatusCollectionView Statuses { get; } = new RecordStatusCollectionView();

        public WorkerCollectionViewModel()
        {
            Items.Filter = IsMatch;
            Items.PageSize = 100;
            Items.SortDescriptions.Add(new SortDescription(nameof(Worker.Similarity), ListSortDirection.Descending));
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
        }

        public async Task InitializeAsync(string firstName, string lastName, string emailAddress)
        {
            items.Clear();
            items.AddRange(await Task.Run(() =>
            {
                string projectPath = Settings.Default.WorkerProjectPath;
                Project project = ProjectExtensions.Open(projectPath);
                View view = project.Views[CoreView.WorkerRosteringForm.Name];
                ICollection<Worker> items = new List<Worker>();
                using (RecordRepository<Worker> repository = new RecordRepository<Worker>(view))
                {
                    foreach (Worker item in repository.Select())
                    {
                        item.SetSimilarity(firstName, lastName, emailAddress);
                        items.Add(item);
                    }
                }
                return items;
            }));
            Items.Refresh();
        }

        private bool IsStatusMatch(Worker item)
        {
            RecordStatus? status = Statuses.CurrentItem?.Value;
            return status == null || item.RECSTATUS == status;
        }

        private bool IsSearchMatch(Worker item)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            if (item.FullName?.Search(searchText) == true)
            {
                return true;
            }
            if (item.EmailAddress?.Search(searchText) == true)
            {
                return true;
            }
            return false;
        }

        private bool IsMatch(object obj)
        {
            Worker item = (Worker)obj;
            return IsStatusMatch(item) && IsSearchMatch(item);
        }
    }
}
