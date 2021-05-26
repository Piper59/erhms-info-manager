using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SearchOption = System.IO.SearchOption;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class AnalysisCollectionViewModel
    {
        public class ItemViewModel : ObservableObject
        {
            private static readonly StringComparer pathComparer = StringComparer.OrdinalIgnoreCase;

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
                return pathComparer.GetHashCode(Value.FullName);
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && pathComparer.Equals(Value.FullName, item.Value.FullName);
            }
        }

        protected abstract Module Module { get; }
        protected abstract string Extension { get; }
        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public PagingListCollectionView Items { get; }

        public FileInfo CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        protected AnalysisCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new PagingListCollectionView(items);
            Items.SortDescriptions.Add(new SortDescription(nameof(FileInfo.Name), ListSortDirection.Ascending));
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, Items.HasCurrent);
            DeleteCommand = new AsyncCommand(DeleteAsync, Items.HasCurrent);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        protected async Task InitializeAsync()
        {
            IEnumerable<FileInfo> values = await Task.Run(() =>
            {
                DirectoryInfo directory = new DirectoryInfo(Project.Location);
                return directory.GetFiles($"*{Extension}", SearchOption.AllDirectories);
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            Items.Refresh();
        }

        protected abstract void CreateCore(View view, string path);

        public async Task CreateAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module);
        }

        public async Task OpenAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module, CurrentValue.FullName);
        }

        public async Task DeleteAsync()
        {
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = ResXResources.Lead_ConfirmFileDeletion;
            dialog.Body = string.Format(ResXResources.Body_ConfirmFileDeletion, CurrentValue.Name);
            dialog.Buttons = DialogButtonCollection.VerbCancel(ResXResources.AccessText_Delete);
            if (dialog.Show() != true)
            {
                return;
            }
            FileSystem.DeleteFile(CurrentValue.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            await RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = ResXResources.Lead_RefreshingFiles;
            await progress.RunAsync(InitializeAsync);
        }
    }
}
