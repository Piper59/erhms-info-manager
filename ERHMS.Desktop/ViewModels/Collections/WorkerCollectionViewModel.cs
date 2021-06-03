using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Common.Linq;
using ERHMS.Desktop.CollectionViews;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class WorkerCollectionViewModel : ObservableObject
    {
        public class ItemViewModel : ObservableObject
        {
            public Worker Value { get; }
            public double Similarity { get; private set; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            public ItemViewModel(Worker value)
            {
                Value = value;
            }

            public void Initialize(string firstName, string lastName, string emailAddress)
            {
                Similarity = Value.GetSimilarity(firstName, lastName, emailAddress);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && Value.Equals(item.Value);
            }
        }

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

        private readonly List<ItemViewModel> items;
        public ICollectionView Items { get; }

        public Worker CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;

        public WorkerCollectionViewModel()
        {
            items = new List<ItemViewModel>();
            Items = new PagingListCollectionView(items)
            {
                Filter = IsMatch,
                PageSize = 100
            };
            Items.SortDescriptions.Add(
                new SortDescription(nameof(ItemViewModel.Similarity),
                ListSortDirection.Descending));
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
        }

        public async Task InitializeAsync(string firstName, string lastName, string emailAddress)
        {
            IEnumerable<Worker> values = await Task.Run(() =>
            {
                string projectPath = Settings.Default.WorkerProjectPath;
                Project project = ProjectExtensions.Open(projectPath);
                View view = project.Views[CoreView.WorkerRosteringForm.Name];
                RecordRepository<Worker> repository = new RecordRepository<Worker>(view);
                return repository.Select().ToList();
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            await Task.Run(() =>
            {
                foreach (ItemViewModel item in items)
                {
                    item.Initialize(firstName, lastName, emailAddress);
                }
            });
            Items.Refresh();
        }

        private bool IsStatusMatch(Worker value)
        {
            return Statuses.CurrentValue == null || value.RECSTATUS == Statuses.CurrentValue;
        }

        private bool IsSearchMatch(Worker value)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            if (value.FullName?.Search(searchText) == true)
            {
                return true;
            }
            if (value.EmailAddress?.Search(searchText) == true)
            {
                return true;
            }
            return false;
        }

        private bool IsMatch(object item)
        {
            Worker value = ((ItemViewModel)item).Value;
            return IsStatusMatch(value) && IsSearchMatch(value);
        }

        public bool MoveCurrentToGlobalRecordId(string globalRecordId)
        {
            foreach (Iterator<ItemViewModel> item in Items.Cast<ItemViewModel>().Iterate())
            {
                if (Record.GlobalRecordIdComparer.Equals(item.Value.Value.GlobalRecordId, globalRecordId))
                {
                    return Items.MoveCurrentToPosition(item.Index);
                }
            }
            return false;
        }
    }
}
