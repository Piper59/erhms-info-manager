using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class ViewCollectionViewModel : CollectionViewModelBase<ViewCollectionViewModel.Item>
    {
        public class Item
        {
            public View Value { get; }
            public int PageCount { get; private set; }
            public int FieldCount { get; private set; }
            public int RecordCount { get; private set; }

            public Item(View value)
            {
                Value = value;
            }

            public void Initialize()
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

        public Project Project { get; }

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DesignCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand RefreshCommand { get; }

        public ViewCollectionViewModel(Project project)
        {
            Project = project;
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, HasCurrentItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, HasCurrentItem);
            DesignCommand = new AsyncCommand(DesignAsync, HasCurrentItem);
            EnterCommand = new AsyncCommand(EnterAsync, HasCurrentItem);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            items.Clear();
            items.AddRange(await Task.Run(() =>
            {
                Project.LoadViews();
                ICollection<Item> items = new List<Item>();
                foreach (View view in Project.Views)
                {
                    Item item = new Item(view);
                    item.Initialize();
                    items.Add(item);
                }
                return items;
            }));
            Items.Refresh();
        }

        public async Task CreateAsync()
        {
            CreateViewViewModel wizard = new CreateViewViewModel(Project);
            if (wizard.Show() == true)
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
            await MainViewModel.Instance.StartEpiInfoAsync(
                Module.MakeView,
                $"/project:{Project.FilePath}",
                $"/view:{CurrentItem.Value.Name}");
        }

        public async Task EnterAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(
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
