using Epi;
using ERHMS.Common.IO;
using ERHMS.Common.Text;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class AssetCollectionViewModel : CollectionViewModel<AssetCollectionViewModel.Item>
    {
        public class Item
        {
            public FileInfo Value { get; }

            public Item(FileInfo value)
            {
                Value = value;
            }

            public override int GetHashCode()
            {
                return Comparers.Path.GetHashCode(Value.FullName);
            }

            public override bool Equals(object obj)
            {
                return obj is Item item && Comparers.Path.Equals(Value.FullName, item.Value.FullName);
            }
        }

        public abstract Module Module { get; }
        public abstract string FileExtension { get; }
        public Project Project { get; }

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        protected AssetCollectionViewModel(Project project)
        {
            Project = project;
            Items.SortDescriptions.Add(new SortDescription(nameof(FileInfo.Name), ListSortDirection.Ascending));
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, HasCurrent);
            DeleteCommand = new AsyncCommand(DeleteAsync, HasCurrent);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        protected async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                List.Clear();
                DirectoryInfo directory = new DirectoryInfo(Project.Location);
                foreach (FileInfo value in directory.EnumerateFiles($"*{FileExtension}"))
                {
                    List.Add(new Item(value));
                }
            });
            Items.Refresh();
        }

        protected abstract Task<CreateAssetViewModel> GetCreateWizardAsync();

        public async Task CreateAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            CreateAssetViewModel wizard = await progress.Run(GetCreateWizardAsync);
            if (wizard.Show() != true)
            {
                return;
            }
            if (wizard.OpenInEpiInfo)
            {
                await Integration.StartWithBackgroundTaskAsync(InitializeAsync, Module, wizard.AssetPath);
            }
            else
            {
                await RefreshAsync();
            }
        }

        public async Task OpenAsync()
        {
            await Integration.StartAsync(Module, CurrentItem.Value.FullName);
        }

        public async Task DeleteAsync()
        {
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = Strings.Lead_ConfirmAssetDeletion;
            dialog.Body = string.Format(Strings.Body_ConfirmAssetDeletion, CurrentItem.Value.Name);
            dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Delete);
            if (dialog.Show() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_DeletingAsset;
            await progress.Run(async () =>
            {
                await Task.Run(() =>
                {
                    FileSystemExtensions.Recycle(CurrentItem.Value.FullName);
                });
                await InitializeAsync();
            });
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_RefreshingAssets;
            await progress.Run(InitializeAsync);
        }
    }
}
