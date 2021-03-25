using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class RecordCollectionViewModel : ViewModel
    {
        public class StatusViewModel : SelectableViewModel
        {
            public static StatusViewModel Undeleted { get; } =
                new StatusViewModel(RecordStatus.Undeleted, ResXResources.RecordStatus_Undeleted);

            public static StatusViewModel Deleted { get; } =
                new StatusViewModel(RecordStatus.Deleted, ResXResources.RecordStatus_Deleted);

            public static StatusViewModel All { get; } = new StatusViewModel(null, ResXResources.RecordStatus_All);

            public static IReadOnlyCollection<StatusViewModel> Instances { get; } = new StatusViewModel[]
            {
                Undeleted,
                Deleted,
                All
            };

            public RecordStatus? Value { get; }
            public string Text { get; }

            private StatusViewModel(RecordStatus? value, string text)
            {
                Value = value;
                Text = text;
            }
        }

        public class ItemViewModel : SelectableViewModel
        {
            public Record Value { get; }

            public ItemViewModel(Record value)
            {
                Value = value;
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

        private static bool IsDisplayable(MetaFieldType fieldType)
        {
            return fieldType == MetaFieldType.RecStatus
                || fieldType == MetaFieldType.GlobalRecordId
                || fieldType.IsPrintable();
        }

        public Project Project => View.Project;
        public View View { get; }

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

        private readonly List<StatusViewModel> statuses;
        public CustomCollectionView<StatusViewModel> Statuses { get; }

        private IReadOnlyCollection<FieldDataRow> fields;
        public IReadOnlyCollection<FieldDataRow> Fields
        {
            get { return fields; }
            private set { SetProperty(ref fields, value); }
        }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }
        public Record SelectedValue => Items.SelectedItem?.Value;

        public ICommand ClearSearchTextCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public RecordCollectionViewModel(View view)
        {
            View = view;
            statuses = StatusViewModel.Instances.ToList();
            Statuses = new CustomCollectionView<StatusViewModel>(statuses);
            Statuses.CurrentChanged += Statuses_CurrentChanged;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items)
            {
                TypedFilter = IsMatch,
                PageSize = 100
            };
            ClearSearchTextCommand = new SyncCommand(ClearSearchText);
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, Items.HasSelection);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasSelection);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, Items.HasSelection);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        private void Statuses_CurrentChanged(object sender, EventArgs e)
        {
            Items.Refresh();
        }

        public async Task InitializeAsync()
        {
            Fields = await Task.Run(() =>
            {
                return View.GetFields()
                    .Where(field => IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex())
                    .ToList();
            });
            IReadOnlyCollection<Record> values = await Task.Run(() =>
            {
                RecordRepository repository = new RecordRepository(View);
                return repository.Select().ToList();
            });
            items.AddRange(values.Select(value => new ItemViewModel(value)).ToList());
            Items.Refresh();
        }

        private bool IsStatusMatch(ItemViewModel item)
        {
            RecordStatus? status = Statuses.SelectedItem?.Value;
            return status == null || status.Value.IsEquivalent(item.Value.RECSTATUS);
        }

        private bool IsSearchMatch(ItemViewModel item)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            foreach (FieldDataRow field in Fields)
            {
                object propertyValue = item.Value.GetProperty(field.Name);
                if (propertyValue == null)
                {
                    continue;
                }
                if (propertyValue.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(ItemViewModel item)
        {
            return IsStatusMatch(item) && IsSearchMatch(item);
        }

        public void ClearSearchText()
        {
            SearchText = "";
        }

        public async Task AddAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{View.Name}",
                "/record:*");
        }

        public async Task EditAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{View.Name}",
                $"/record:{SelectedValue.UniqueKey}");
        }

        private async Task SetDeletedAsync(string lead, bool deleted)
        {
            try
            {
                await ServiceLocator.Resolve<IProgressService>().RunAsync(
                    lead,
                    token =>
                    {
                        RecordRepository repository = new RecordRepository(View);
                        foreach (ItemViewModel item in Items.SelectedItems)
                        {
                            token.ThrowIfCancellationRequested();
                            repository.SetDeleted(item.Value, deleted);
                        }
                    });
            }
            catch (OperationCanceledException) { }
            Items.Refresh();
        }

        public async Task DeleteAsync()
        {
            await SetDeletedAsync(ResXResources.Lead_DeletingRecords, true);
        }

        public async Task UndeleteAsync()
        {
            await SetDeletedAsync(ResXResources.Lead_UndeletingRecords, false);
        }

        public async Task RefreshAsync()
        {
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_RefreshingRecords,
                async () =>
                {
                    items.Clear();
                    await InitializeAsync();
                });
        }
    }
}
