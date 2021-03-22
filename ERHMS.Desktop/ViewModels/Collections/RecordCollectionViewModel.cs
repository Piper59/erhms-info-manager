using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class RecordCollectionViewModel : ViewModel
    {
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
        public IReadOnlyCollection<FieldDataRow> Fields { get; private set; }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ItemViewModel SelectedItem => Items.SelectedItem;
        public Record SelectedValue => SelectedItem?.Value;

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndeleteCommand { get; }

        public RecordCollectionViewModel(View view, IEnumerable<Record> values)
        {
            View = view;
            items = new List<ItemViewModel>(values.Select(value => new ItemViewModel(value)));
            Items = new CustomCollectionView<ItemViewModel>(items);
            CreateCommand = new AsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, Items.HasSelection);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasSelection);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, Items.HasSelection);
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
        }

        public async Task CreateAsync()
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
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                lead,
                () =>
                {
                    RecordRepository recordRepository = new RecordRepository(View);
                    foreach (ItemViewModel item in Items.SelectedItems)
                    {
                        recordRepository.SetDeleted(item.Value, deleted);
                    }
                });
        }

        public async Task DeleteAsync()
        {
            await SetDeletedAsync(ResXResources.Lead_DeletingRecords, true);
        }

        public async Task UndeleteAsync()
        {
            await SetDeletedAsync(ResXResources.Lead_UndeletingRecords, false);
        }
    }
}
