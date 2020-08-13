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
    public partial class ProjectViewModel
    {
        public class ViewsChildViewModel : ObservableObject
        {
            public class Item : ObservableObject, ISelectable
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

                public Item(Epi.View view)
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

                public override int GetHashCode()
                {
                    return View.Id.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    return obj is Item item && item.View.Id == View.Id;
                }
            }

            public Project Project { get; }

            private readonly CustomCollectionView<Item> items;
            public ICustomCollectionView<Item> Items => items;

            public ICommand CustomizeCommand { get; }
            public ICommand ViewDataCommand { get; }
            public ICommand EnterDataCommand { get; }
            public ICommand ExportDataCommand { get; }
            public ICommand ImportDataCommand { get; }
            public ICommand AnalyzeCommand { get; }
            public ICommand DeleteCommand { get; }

            public ViewsChildViewModel(Project project)
            {
                Project = project;
                items = new CustomCollectionView<Item>();
                RefreshData();
                CustomizeCommand = new AsyncCommand(CustomizeAsync, items.HasSelectedItem, ErrorBehavior.Raise);
                ViewDataCommand = new AsyncCommand(ViewDataAsync, items.HasSelectedItem, ErrorBehavior.Raise);
                EnterDataCommand = new AsyncCommand(EnterDataAsync, items.HasSelectedItem, ErrorBehavior.Raise);
                ExportDataCommand = new SyncCommand(ExportData, items.HasSelectedItem, ErrorBehavior.Raise);
                ImportDataCommand = new SyncCommand(ImportData, items.HasSelectedItem, ErrorBehavior.Raise);
                AnalyzeCommand = new SyncCommand(Analyze, items.HasSelectedItem, ErrorBehavior.Raise);
                DeleteCommand = new SyncCommand(Delete, items.HasSelectedItem, ErrorBehavior.Raise);
            }

            public void RefreshData()
            {
                Project.LoadViews();
                items.Source.Clear();
                items.Source.AddRange(Project.Views.Cast<Epi.View>().Select(view => new Item(view)));
            }

            public void RefreshView()
            {
                items.Refresh();
            }

            public async Task CustomizeAsync()
            {
                Epi.View view = items.SelectedItem.View;
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
                    content = new ViewViewModel(Project, items.SelectedItem.View);
                });
                MainViewModel.Current.Content = content;
            }

            public async Task EnterDataAsync()
            {
                Epi.View view = items.SelectedItem.View;
                await MainViewModel.Current.StartEpiInfoAsync(
                    Module.Enter,
                    $"/project:{view.Project.FilePath}",
                    $"/view:{view.Name}",
                    "/record:*");
            }

            public void ExportData()
            {
                throw new System.NotImplementedException();
            }

            public void ImportData()
            {
                throw new System.NotImplementedException();
            }

            public void Analyze()
            {
                throw new System.NotImplementedException();
            }

            public void Delete()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
