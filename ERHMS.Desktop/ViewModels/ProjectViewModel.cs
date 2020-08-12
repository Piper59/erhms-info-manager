using Epi.Fields;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewItem : ObservableObject, ISelectable
        {
            public Epi.View View { get; }
            public string Title { get; }
            public int RecordCount { get; }

            private bool isSelected;
            public bool IsSelected
            {
                get { return isSelected; }
                set { SetProperty(ref isSelected, value); }
            }

            public ViewItem(Epi.View view)
            {
                View = view;
                Title = view.Pages[0].Fields.OfType<LabelField>()
                    .OrderBy(field => field.TabIndex)
                    .FirstOrDefault(field => field.Name.StartsWith("Title", StringComparison.OrdinalIgnoreCase))
                    ?.PromptText;
                RecordRepository repository = new RecordRepository(view);
                if (repository.TableExists())
                {
                    RecordCount = repository.Count(repository.GetWhereDeletedClause(false));
                }
            }

            public override int GetHashCode() => View.Id.GetHashCode();

            public override bool Equals(object obj)
            {
                return obj is ViewItem viewItem && viewItem.View.Id == View.Id;
            }
        }

        public Project Project { get; }

        private readonly CustomCollectionView<ViewItem> viewItems;
        public ICustomCollectionView<ViewItem> ViewItems => viewItems;

        public ICommand RefreshCommand { get; }
        public ICommand CustomizeCommand { get; }
        public ICommand ViewDataCommand { get; }
        public ICommand EnterDataCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            viewItems = new CustomCollectionView<ViewItem>();
            RefreshInternal();
            viewItems.Refresh();
            RefreshCommand = new AsyncCommand(RefreshAsync, Command.Always, ErrorBehavior.Raise);
            CustomizeCommand = new AsyncCommand(CustomizeAsync, viewItems.HasSelectedItem, ErrorBehavior.Raise);
            ViewDataCommand = new AsyncCommand(ViewDataAsync, viewItems.HasSelectedItem, ErrorBehavior.Raise);
            EnterDataCommand = new AsyncCommand(EnterDataAsync, viewItems.HasSelectedItem, ErrorBehavior.Raise);
        }

        private void RefreshInternal()
        {
            viewItems.Source.Clear();
            viewItems.Source.AddRange(Project.Views.Cast<Epi.View>().Select(view => new ViewItem(view)));
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName, false);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                RefreshInternal();
            });
            viewItems.Refresh();
        }

        public async Task CustomizeAsync()
        {
            Epi.View view = viewItems.SelectedItem.View;
            await MainViewModel.Current.StartEpiInfoAsync(
                Module.MakeView,
                $"/project:{view.Project.FilePath}",
                $"/view:{view.Name}");
        }

        public async Task ViewDataAsync()
        {
            ViewViewModel content = null;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningViewTaskName, false);
            await progress.RunAsync(() =>
            {
                content = new ViewViewModel(Project, viewItems.SelectedItem.View);
            });
            MainViewModel.Current.Content = content;
        }

        public async Task EnterDataAsync()
        {
            Epi.View view = viewItems.SelectedItem.View;
            await MainViewModel.Current.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{view.Project.FilePath}",
                $"/view:{view.Name}",
                "/record:*");
        }
    }
}
