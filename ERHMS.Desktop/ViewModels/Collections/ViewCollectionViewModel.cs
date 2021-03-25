using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
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
                        RecordRepository repository = new RecordRepository(Value);
                        RecordCount = repository.CountByDeleted(false);
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
        public View SelectedValue => Items.SelectedItem?.Value;

        public ICommand CreateCommand { get; }
        public ICommand CustomizeCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ViewDataCommand { get; }
        public ICommand EnterDataCommand { get; }
        public ICommand ImportDataCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand RefreshCommand { get; }

        public ViewCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items);
            CreateCommand = Command.Null;
            CustomizeCommand = new AsyncCommand(CustomizeAsync, Items.HasSelection);
            DeleteCommand = Command.Null;
            ViewDataCommand = new AsyncCommand(ViewDataAsync, Items.HasSelection);
            EnterDataCommand = new AsyncCommand(EnterDataAsync, Items.HasSelection);
            ImportDataCommand = Command.Null;
            ExportDataCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            IReadOnlyCollection<View> values = await Task.Run(() =>
            {
                return Project.Views.Cast<View>().ToList();
            });
            items.AddRange(values.Select(value => new ItemViewModel(value)).ToList());
            foreach (ItemViewModel item in items)
            {
                await item.InitializeAsync();
            }
            Items.Refresh();
        }

        public async Task CustomizeAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.MakeView,
                $"/project:{Project.FilePath}",
                $"/view:{SelectedValue.Name}");
        }

        public async Task ViewDataAsync()
        {
            await MainViewModel.Instance.GoToViewAsync(Project.FilePath, SelectedValue.Name);
        }

        public async Task EnterDataAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{SelectedValue.Name}",
                "/record:*");
        }

        public async Task RefreshAsync()
        {
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_RefreshingViews,
                async () =>
                {
                    await Task.Run(() =>
                    {
                        Project.LoadViews();
                    });
                    items.Clear();
                    await InitializeAsync();
                });
        }
    }
}
