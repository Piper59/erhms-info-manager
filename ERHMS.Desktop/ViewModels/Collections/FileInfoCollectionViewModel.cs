using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
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
        public class ItemViewModel : ObservableObject, ISelectable
        {
            private static StringComparer PathComparer => StringComparer.OrdinalIgnoreCase;

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
                return PathComparer.GetHashCode(Value.FullName);
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && PathComparer.Equals(Value.FullName, item.Value.FullName);
            }
        }

        protected abstract string Extension { get; }
        protected abstract string RefreshingLead { get; }
        public Project Project { get; }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        protected FileInfoCollectionViewModel(Project project)
        {
            Project = project;
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items);
            CreateCommand = Command.Null;
            OpenCommand = Command.Null;
            DeleteCommand = Command.Null;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        protected async Task InitializeAsync()
        {
            IReadOnlyCollection<FileInfo> values = await Task.Run(() =>
            {
                DirectoryInfo directory = new DirectoryInfo(Project.Location);
                return directory.GetFiles($"*{Extension}", SearchOption.AllDirectories);
            });
            items.Clear();
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            Items.Refresh();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = RefreshingLead;
            await progress.RunAsync(InitializeAsync);
        }
    }

    public class CanvasCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<CanvasCollectionViewModel> CreateAsync(Project project)
        {
            CanvasCollectionViewModel result = new CanvasCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override string Extension => ".cvs7";
        protected override string RefreshingLead => ResXResources.Lead_RefreshingCanvases;

        private CanvasCollectionViewModel(Project project)
            : base(project) { }
    }

    public class ProgramCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<ProgramCollectionViewModel> CreateAsync(Project project)
        {
            ProgramCollectionViewModel result = new ProgramCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override string Extension => ".pgm7";
        protected override string RefreshingLead => ResXResources.Lead_RefreshingPrograms;

        private ProgramCollectionViewModel(Project project)
            : base(project) { }
    }

    public class MapCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<MapCollectionViewModel> CreateAsync(Project project)
        {
            MapCollectionViewModel result = new MapCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override string Extension => ".map7";
        protected override string RefreshingLead => ResXResources.Lead_RefreshingMaps;

        private MapCollectionViewModel(Project project)
            : base(project) { }
    }
}
