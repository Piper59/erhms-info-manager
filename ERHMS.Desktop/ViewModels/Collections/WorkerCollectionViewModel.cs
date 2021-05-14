using Epi;
using ERHMS.Common.ComponentModel;
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

        public RecordStatusCollectionView Statuses { get; } = new RecordStatusCollectionView();

        private readonly List<ItemViewModel> items;
        public PagingListCollectionView Items { get; }

        public Worker CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;

        private WorkerCollectionViewModel(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
            items = new List<ItemViewModel>();
            Items = new PagingListCollectionView(items)
            {
                Filter = IsMatch,
                PageSize = 100
            };
            Items.SortDescriptions.Add(
                new SortDescription(nameof(ItemViewModel.Similarity),
                ListSortDirection.Descending));
        }

        private async Task InitializeAsync()
        {
            IEnumerable<Worker> values = await Task.Run(() =>
            {
                Project project = ProjectExtensions.Open(Settings.Default.WorkerProjectPath);
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
                    item.Initialize(FirstName, LastName, EmailAddress);
                }
            });
            Items.Refresh();
        }

        private bool IsStatusMatch(Worker value)
        {
            return value.RECSTATUS == Statuses.CurrentValue;
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
            int itemIndex = -1;
            foreach (ItemViewModel item in Items.Cast<ItemViewModel>())
            {
                itemIndex++;
                if (Record.GlobalRecordIdComparer.Equals(item.Value.GlobalRecordId, globalRecordId))
                {
                    return Items.MoveCurrentToPosition(itemIndex);
                }
            }
            return false;
        }
    }
}
