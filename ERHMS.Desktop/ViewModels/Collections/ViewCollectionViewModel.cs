using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
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
    public class ViewCollectionViewModel
    {
        public class ItemViewModel : ObservableObject, ISelectable
        {
            public View Value { get; }
            public int PageCount { get; private set; }
            public int FieldCount { get; private set; }
            public int RecordCount { get; private set; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            public ItemViewModel(View value)
            {
                Value = value;
            }

            public void Initialize()
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

        public static async Task<ViewCollectionViewModel> CreateAsync(Project project)
        {
            ViewCollectionViewModel result = new ViewCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ICommand CreateCommand { get; }
        public ICommand CustomizeCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ViewRecordsCommand { get; }
        public ICommand EnterRecordsCommand { get; }
        public ICommand ImportRecordsCommand { get; }
        public ICommand ExportRecordsCommand { get; }
        public ICommand RefreshCommand { get; }

        private ViewCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items);
            CreateCommand = Command.Null;
            CustomizeCommand = new AsyncCommand(CustomizeAsync, Items.HasSelection);
            DeleteCommand = Command.Null;
            ViewRecordsCommand = new AsyncCommand(ViewRecordsAsync, Items.HasSelection);
            EnterRecordsCommand = new AsyncCommand(EnterRecordsAsync, Items.HasSelection);
            ImportRecordsCommand = Command.Null;
            ExportRecordsCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        private async Task InitializeAsync()
        {
            IReadOnlyCollection<View> values = await Task.Run(() =>
            {
                Project.LoadViews();
                return Project.Views.Cast<View>().ToList();
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            await Task.Run(() =>
            {
                foreach (ItemViewModel item in items)
                {
                    item.Initialize();
                }
            });
            Items.Refresh();
        }

        public async Task CustomizeAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.MakeView,
                $"/project:{Project.FilePath}",
                $"/view:{Items.SelectedItem.Value.Name}");
        }

        public async Task ViewRecordsAsync()
        {
            await MainViewModel.Instance.GoToViewAsync(Project.FilePath, Items.SelectedItem.Value.Name);
        }

        public async Task EnterRecordsAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{Items.SelectedItem.Value.Name}",
                "/record:*");
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_RefreshingViews;
            await progress.RunAsync(InitializeAsync);
        }
    }
}
