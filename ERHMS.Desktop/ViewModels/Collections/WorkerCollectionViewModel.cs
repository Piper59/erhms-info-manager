using Epi;
using ERHMS.Common;
using ERHMS.Desktop.CollectionViews;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class WorkerCollectionViewModel : ObservableObject
    {
        public class ItemViewModel : ObservableObject, ISelectable
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
        public CustomCollectionView<ItemViewModel> Items { get; }

        private WorkerCollectionViewModel(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Statuses.CurrentChanged += Statuses_CurrentChanged;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items)
            {
                TypedFilter = IsMatch,
                PageSize = 100
            };
            Items.SortDescriptions.Add(
                new SortDescription(nameof(ItemViewModel.Similarity),
                ListSortDirection.Descending));
        }

        private void Statuses_CurrentChanged(object sender, EventArgs e)
        {
            Items.Refresh();
        }

        private async Task InitializeAsync()
        {
            IReadOnlyCollection<Worker> values = await Task.Run(() =>
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
            RecordStatus? status = Statuses.SelectedItem?.Value;
            return status == null || value.RECSTATUS.IsEquivalent(status.Value);
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

        private bool IsMatch(ItemViewModel item)
        {
            return IsStatusMatch(item.Value) && IsSearchMatch(item.Value);
        }
    }
}
