using Epi;
using Epi.Data.Services;
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
    public class RecordCollectionViewModel : CollectionViewModelBase<Record>
    {
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
            Items.Filter = IsMatch;
            Items.PageSize = 100;
            Statuses.CurrentChanged += (sender, e) => Items.Refresh();
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, HasCurrentItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, HasSelectedItems);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, HasSelectedItems);
            ImportCommand = Command.Null;
            ExportCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            Fields = await Task.Run(() =>
            {
                return ((MetadataDbProvider)Project.Metadata).GetFieldDataTableForView(View.Id)
                    .Where(field => field.Position.GetValueOrDefault() == 0 && IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex())
                    .ToList();
            });
            items.Clear();
            items.AddRange(await Task.Run(() =>
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
            }));
            Items.Refresh();
        }

        private bool IsStatusMatch(Record item)
        {
            RecordStatus? status = Statuses.CurrentItem?.Value;
            return status == null || item.RECSTATUS == status;
        }

        private bool IsSearchMatch(Record item)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return true;
            }
            foreach (FieldDataRow field in fields)
            {
                object value = item.GetProperty(field.Name);
                if (value?.ToString().Search(SearchText) ?? false)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(object obj)
        {
            Record item = (Record)obj;
            return IsStatusMatch(item) && IsSearchMatch(item);
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
                $"/record:{CurrentItem.UniqueKey}");
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
                        foreach (Record item in SelectedItems)
                        {
                            progress.ThrowIfCancellationRequested();
                            repository.SetDeleted(item, deleted);
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
