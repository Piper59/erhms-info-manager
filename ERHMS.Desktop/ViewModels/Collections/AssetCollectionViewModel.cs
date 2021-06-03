using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Common.Text;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class AssetCollectionViewModel
    {
        public class ItemViewModel : ObservableObject
        {
            public FileInfo Value { get; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            public ItemViewModel(FileInfo value)
            {
                Value = value;
            }

            public override int GetHashCode()
            {
                return Comparers.Path.GetHashCode(Value.FullName);
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && Comparers.Path.Equals(Value.FullName, item.Value.FullName);
            }
        }

        protected abstract Module Module { get; }
        protected abstract string FileExtension { get; }
        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public ICollectionView Items { get; }

        public FileInfo CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        protected AssetCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new ListCollectionView(items);
            Items.SortDescriptions.Add(new SortDescription(nameof(FileInfo.Name), ListSortDirection.Ascending));
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, Items.HasCurrent);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasCurrent);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            IEnumerable<FileInfo> values = await Task.Run(() =>
            {
                DirectoryInfo directory = new DirectoryInfo(Project.Location);
                return directory.GetFiles($"*{FileExtension}");
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
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
            await MainViewModel.Instance.StartEpiInfoAsync(Module, CurrentValue.FullName);
        }

        public async Task DeleteAsync()
        {
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = ResXResources.Lead_ConfirmAssetDeletion;
            dialog.Body = string.Format(ResXResources.Body_ConfirmAssetDeletion, CurrentValue.Name);
            dialog.Buttons = DialogButtonCollection.ActionOrCancel(ResXResources.AccessText_Delete);
            if (dialog.Show() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_DeletingAsset;
            await progress.Run(async () =>
            {
                await Task.Run(() =>
                {
                    FileSystem.DeleteFile(
                        CurrentValue.FullName,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);
                });
                await InitializeAsync();
            });
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_RefreshingAssets;
            await progress.Run(InitializeAsync);
        }
    }
}
