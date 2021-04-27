using Epi;
using ERHMS.Common;
using ERHMS.Desktop.CollectionViews;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure;
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
    public class RecordCollectionViewModel : ObservableObject
    {
        public class ItemViewModel : ObservableObject, ISelectable
        {
            public Record Value { get; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

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

        public static async Task<RecordCollectionViewModel> CreateAsync(View view)
        {
            RecordCollectionViewModel result = new RecordCollectionViewModel(view);
            await result.InitializeAsync();
            return result;
        }

        private static bool IsDisplayable(MetaFieldType fieldType)
        {
            if (!fieldType.IsPrintable())
            {
                return false;
            }
            if (fieldType.IsMetadata())
            {
                return fieldType == MetaFieldType.RecStatus || fieldType == MetaFieldType.GlobalRecordId;
            }
            return true;
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

        public RecordStatusCollectionView Statuses { get; } = new RecordStatusCollectionView();

        private IReadOnlyList<FieldDataRow> fields;
        public IReadOnlyList<FieldDataRow> Fields
        {
            get { return fields; }
            private set { SetProperty(ref fields, value); }
        }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndeleteCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RefreshCommand { get; }

        private RecordCollectionViewModel(View view)
        {
            View = view;
            Statuses.CurrentChanged += Statuses_CurrentChanged;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items)
            {
                TypedFilter = IsMatch,
                PageSize = 100
            };
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, Items.HasSelection);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasSelection);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, Items.HasSelection);
            ImportCommand = Command.Null;
            ExportCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        private void Statuses_CurrentChanged(object sender, EventArgs e)
        {
            Items.Refresh();
        }

        private async Task InitializeAsync()
        {
            Fields = await Task.Run(() =>
            {
                return View.GetFields()
                    .Where(field => IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabOrder())
                    .ToList();
            });
            IReadOnlyCollection<Record> values = await Task.Run(() =>
            {
                RecordRepository repository = new RecordRepository(View);
                return repository.Select().ToList();
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            Items.Refresh();
        }

        private bool IsStatusMatch(Record value)
        {
            RecordStatus? status = Statuses.SelectedItem?.Value;
            return status == null || value.RECSTATUS.IsEquivalent(status.Value);
        }

        private bool IsSearchMatch(Record value)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            foreach (FieldDataRow field in Fields)
            {
                object propertyValue = value.GetProperty(field.Name);
                if (propertyValue != null && propertyValue.ToString().Search(searchText))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(ItemViewModel item)
        {
            return IsStatusMatch(item.Value) && IsSearchMatch(item.Value);
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
                $"/record:{Items.SelectedItem.Value.UniqueKey}");
        }

        private async Task SetDeletedAsync(string title, bool deleted)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = title;
            try
            {
                await progress.RunAsync(token =>
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
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_RefreshingRecords;
            await progress.RunAsync(InitializeAsync);
        }
    }
}
