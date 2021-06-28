using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class AssetCollectionViewModel : CollectionViewModelBase<FileInfo>
    {
        protected abstract Module Module { get; }
        protected abstract string FileExtension { get; }
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
            OpenCommand = new AsyncCommand(OpenAsync, HasCurrentItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, HasCurrentItem);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            items.Clear();
            items.AddRange(await Task.Run(() =>
            {
                DirectoryInfo directory = new DirectoryInfo(Project.Location);
                return directory.GetFiles($"*{FileExtension}");
            }));
            Items.Refresh();
        }

        protected abstract CreateAssetViewModel GetCreateWizard();

        public async Task CreateAsync()
        {
            CreateAssetViewModel wizard = GetCreateWizard();
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            await progress.Run(wizard.InitializeAsync);
            if (wizard.Show() == true)
            {
                await RefreshAsync();
            }
        }

        public async Task OpenAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module, CurrentItem.FullName);
        }

        public async Task DeleteAsync()
        {
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = Strings.Lead_ConfirmAssetDeletion;
            dialog.Body = string.Format(Strings.Body_ConfirmAssetDeletion, CurrentItem.Name);
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
                    FileSystem.DeleteFile(
                        CurrentItem.FullName,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);
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
