using Epi;
using Epi.Data.Services;
using ERHMS.Common.ComponentModel;
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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class RecordCollectionViewModel : ObservableObject
    {
        public class ItemViewModel : ObservableObject
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

        private static bool IsDisplayable(MetaFieldType fieldType)
        {
            if (fieldType.IsMetadata())
            {
                switch (fieldType)
                {
                    case MetaFieldType.RecStatus:
                    case MetaFieldType.GlobalRecordId:
                        return true;
                    default:
                        return false;
                }
            }
            return fieldType.IsPrintable();
        }

        public View View { get; }
        public Project Project => View.Project;

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

        private IEnumerable<FieldDataRow> fields;
        public IEnumerable<FieldDataRow> Fields
        {
            get { return fields; }
            private set { SetProperty(ref fields, value); }
        }

        private readonly List<ItemViewModel> items;
        public ICollectionView Items { get; }

        public Record CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;
        public IEnumerable<Record> SelectedValues => Items.Cast<ItemViewModel>()
            .Where(item => item.Selected)
            .Select(item => item.Value);

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndeleteCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RefreshCommand { get; }

        public RecordCollectionViewModel(View view)
        {
            View = view;
            items = new List<ItemViewModel>();
            Items = new PagingListCollectionView(items)
            {
                Filter = IsMatch,
                PageSize = 100
            };
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, Items.HasCurrent);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasCurrent);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, Items.HasCurrent);
            ImportCommand = Command.Null;
            ExportCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            Fields = await Task.Run(() =>
            {
                return ((MetadataDbProvider)Project.Metadata).GetFieldDataTableForView(View.Id)
                    .Where(field => IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex())
                    .ToList();
            });
            IEnumerable<Record> values = await Task.Run(() =>
            {
                if (Project.CollectedData.TableExists(View.TableName))
                {
                    using (RecordRepository repository = new RecordRepository(View))
                    {
                        return repository.Select().ToList();
                    }
                }
                else
                {
                    return Enumerable.Empty<Record>();
                }
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            Items.Refresh();
        }

        private bool IsStatusMatch(Record value)
        {
            return Statuses.CurrentValue == null || value.RECSTATUS == Statuses.CurrentValue;
        }

        private bool IsSearchMatch(Record value)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            foreach (FieldDataRow field in fields)
            {
                object propertyValue = value.GetProperty(field.Name);
                if (propertyValue?.ToString().Search(SearchText) ?? false)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(object item)
        {
            Record value = ((ItemViewModel)item).Value;
            return IsStatusMatch(value) && IsSearchMatch(value);
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
                $"/record:{CurrentValue.UniqueKey}");
        }

        private async Task SetDeletedAsync(string lead, bool deleted)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = lead;
            progress.CanBeCanceled = true;
            await progress.Run(() =>
            {
                try
                {
                    using (RecordRepository repository = new RecordRepository(View))
                    {
                        foreach (Record value in SelectedValues)
                        {
                            progress.ThrowIfCancellationRequested();
                            repository.SetDeleted(value, deleted);
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
            Items.Refresh();
        }

        public async Task DeleteAsync()
        {
            await SetDeletedAsync(Strings.Lead_DeletingRecords, true);
        }

        public async Task UndeleteAsync()
        {
            await SetDeletedAsync(Strings.Lead_UndeletingRecords, false);
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_RefreshingRecords;
            await progress.Run(InitializeAsync);
        }
    }
}
