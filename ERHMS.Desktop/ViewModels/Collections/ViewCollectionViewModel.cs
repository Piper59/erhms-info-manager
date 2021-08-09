using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class ViewCollectionViewModel : CollectionViewModel<ViewCollectionViewModel.Item>
    {
        public class Item
        {
            public static async Task<Item> CreateAsync(View value)
            {
                Item result = new Item(value);
                await result.InitializeAsync();
                return result;
            }

            public View Value { get; }
            public int PageCount { get; private set; }
            public int FieldCount { get; private set; }
            public int RecordCount { get; private set; }

            private Item(View value)
            {
                Value = value;
            }

            private async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    PageCount = Value.Pages.Count;
                    FieldCount = Value.Fields.InputFields.Count;
                    if (Value.Project.CollectedData.TableExists(Value.TableName))
                    {
                        using (RecordRepository repository = new RecordRepository(Value))
                        {
                            RecordCount = repository.CountByDeleted(false);
                        }
                    }
                    else
                    {
                        RecordCount = 0;
                    }
                });
            }

            public override int GetHashCode()
            {
                return Value.Id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is Item item && Value.Id == item.Value.Id;
            }
        }

        public static async Task<ViewCollectionViewModel> CreateAsync(Project project)
        {
            ViewCollectionViewModel result = new ViewCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        public Project Project { get; }

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DesignCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand RefreshCommand { get; }

        private ViewCollectionViewModel(Project project)
        {
            Project = project;
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, HasCurrent);
            DeleteCommand = new AsyncCommand(DeleteAsync, HasCurrent);
            DesignCommand = new AsyncCommand(DesignAsync, HasCurrent);
            EnterCommand = new AsyncCommand(EnterAsync, HasCurrent);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                List.Clear();
                Project.LoadViews();
                Task<Item>[] tasks = Project.Views.Cast<View>()
                    .Select(value => Item.CreateAsync(value))
                    .ToArray();
                Task.WaitAll(tasks);
                foreach (Task<Item> task in tasks)
                {
                    List.Add(task.Result);
                }
            });
            Items.Refresh();
        }

        public async Task CreateAsync()
        {
            CreateViewViewModel wizard = new CreateViewViewModel(Project);
            if (wizard.Show() != true)
            {
                return;
            }
            if (wizard.OpenInEpiInfo)
            {
                await Integration.StartWithBackgroundTaskAsync(
                    InitializeAsync,
                    Module.MakeView,
                    $"/project:{wizard.View.Project.FilePath}",
                    $"/view:{wizard.View.Name}");
            }
            else
            {
                await RefreshAsync();
            }
        }

        public async Task OpenAsync()
        {
            await MainViewModel.Instance.GoToViewAsync(() => Task.FromResult(CurrentItem.Value));
        }

        public async Task DeleteAsync()
        {
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = Strings.Lead_ConfirmViewDeletion;
            dialog.Body = string.Format(Strings.Body_ConfirmViewDeletion, CurrentItem.Value.Name);
            dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Delete);
            if (dialog.Show() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_DeletingView;
            await progress.Run(async () =>
            {
                await Task.Run(() =>
                {
                    Project.DeleteView(CurrentItem.Value);
                });
                await InitializeAsync();
            });
        }

        public async Task DesignAsync()
        {
            await Integration.StartAsync(
                Module.MakeView,
                $"/project:{Project.FilePath}",
                $"/view:{CurrentItem.Value.Name}");
        }

        public async Task EnterAsync()
        {
            await Integration.StartAsync(
                Module.Enter,
                $"/project:{Project.FilePath}",
                $"/view:{CurrentItem.Value.Name}",
                "/record:*");
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_RefreshingViews;
            await progress.Run(InitializeAsync);
        }
    }
}
