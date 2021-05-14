using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class FileInfoCollectionViewModel
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
        protected abstract string RefreshingLead { get; }
        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public PagingListCollectionView Items { get; }

        public FileInfo CurrentValue => ((ItemViewModel)Items.CurrentItem)?.Value;

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        protected FileInfoCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new PagingListCollectionView(items);
            CreateCommand = new AsyncCommand(CreateAsync);
            OpenCommand = new AsyncCommand(OpenAsync, Items.HasCurrent);
            DeleteCommand = Command.Null;
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

        public async Task CreateAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module);
        }

        public async Task OpenAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module, CurrentValue.FullName);
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = RefreshingLead;
            await progress.RunAsync(InitializeAsync);
        }
    }
}
