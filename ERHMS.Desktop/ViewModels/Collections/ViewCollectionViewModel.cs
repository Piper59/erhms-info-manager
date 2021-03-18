using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class ViewCollectionViewModel : ViewModel
    {
        public class ItemViewModel : SelectableViewModel
        {
            public View Value { get; }
            public int PageCount { get; private set; }
            public int FieldCount { get; private set; }
            public int RecordCount { get; private set; }

            public ItemViewModel(View value)
            {
                Value = value;
            }

            public async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    PageCount = Value.Pages.Count;
                    FieldCount = Value.Fields.InputFields.Count;
                    if (Value.TableExists())
                    {
                        RecordRepository recordRepository = new RecordRepository(Value);
                        RecordCount = recordRepository.CountByDeleted(false);
                    }
                    else
                    {
                        RecordCount = 0;
                    }
                });
            }

            public override int GetHashCode()
            {
                return HashCodeCalculator.GetHashCode(Value.Project.Id, Value.Id);
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item
                    && Value.Project.Id == item.Value.Project.Id
                    && Value.Id == item.Value.Id;
            }
        }

        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ItemViewModel SelectedItem => Items.SelectedItem;
        public View SelectedValue => SelectedItem?.Value;

        public ICommand CreateCommand { get; }
        public ICommand CustomizeCommand { get; }
        public ICommand EnterDataCommand { get; }
        public ICommand ViewDataCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }
        public ICommand DeleteCommand { get; }

        public ViewCollectionViewModel(Project project, IEnumerable<View> values)
        {
            Project = project;
            items = new List<ItemViewModel>(values.Select(value => new ItemViewModel(value)));
            Items = new CustomCollectionView<ItemViewModel>(items);
            CustomizeCommand = new AsyncCommand(CustomizeAsync, Items.HasSelection);
            EnterDataCommand = new AsyncCommand(EnterDataAsync, Items.HasSelection);
            ViewDataCommand = new AsyncCommand(ViewDataAsync, Items.HasSelection);
        }

        public async Task InitializeAsync()
        {
            foreach (ItemViewModel item in items)
            {
                await item.InitializeAsync();
            }
        }

        public async Task CustomizeAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.MakeView,
                $"/project:{Project.FilePath}",
                $"/view:{SelectedValue.Name}");
        }

        public async Task EnterDataAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{SelectedValue.Name}",
                "/record:*");
        }

        public async Task ViewDataAsync()
        {
            await MainViewModel.Instance.GoToViewAsync(Task.Run(() =>
            {
                SelectedValue.LoadFields();
                return SelectedValue;
            }));
        }
    }
}
