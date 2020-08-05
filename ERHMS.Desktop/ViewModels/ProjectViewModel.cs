using Epi.Fields;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewItemViewModel : ObservableObject
        {
            public static IEnumerable<ViewItemViewModel> GetAll(Project project)
            {
                ISet<string> tableNames = new HashSet<string>(project.Database.GetTableNames(), StringComparer.OrdinalIgnoreCase);
                foreach (Epi.View view in project.Views)
                {
                    ViewItemViewModel viewItem = new ViewItemViewModel(view)
                    {
                        Title = view.Pages[0].Fields.OfType<LabelField>()
                            .OrderBy(field => field.TabIndex)
                            .FirstOrDefault(field => field.Name.StartsWith("Title", StringComparison.OrdinalIgnoreCase))
                            ?.PromptText
                    };
                    if (tableNames.Contains(view.TableName))
                    {
                        RecordRepository repository = new RecordRepository(project.Database, view);
                        viewItem.RecordCount = repository.Count(repository.GetWhereDeletedClause(false));
                    }
                    yield return viewItem;
                }
            }

            public Epi.View View { get; }
            public string Title { get; private set; }
            public int RecordCount { get; private set; }

            private ViewItemViewModel(Epi.View view)
            {
                View = view;
            }

            public override int GetHashCode()
            {
                return View.Id;
            }

            public override bool Equals(object obj)
            {
                return obj is ViewItemViewModel viewItem && viewItem.View.Id == View.Id;
            }
        }

        public Project Project { get; }

        private ICollection<ViewItemViewModel> viewItems;
        public ICollection<ViewItemViewModel> ViewItems
        {
            get { return viewItems; }
            set { SetProperty(ref viewItems, value); }
        }

        private ViewItemViewModel selectedViewItem;
        public ViewItemViewModel SelectedViewItem
        {
            get { return selectedViewItem; }
            set { SetProperty(ref selectedViewItem, value); }
        }

        public Command RefreshCommand { get; }
        public Command OpenViewCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            OpenViewCommand = new SimpleAsyncCommand<Epi.View>(OpenViewAsync);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedViewItem))
            {
                if (SelectedViewItem != null)
                {
                    OpenViewCommand.Execute(SelectedViewItem.View);
                }
            }
        }

        private void RefreshInternal()
        {
            viewItems = ViewItemViewModel.GetAll(Project).ToList();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                RefreshInternal();
            });
            OnPropertyChanged(nameof(ViewItems));
        }

        public async Task OpenViewAsync(Epi.View view)
        {
            ViewViewModel content = null;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningViewTaskName);
            await progress.RunAsync(() =>
            {
                content = new ViewViewModel(this, view);
            });
            MainViewModel.Current.Content = content;
        }
    }
}
