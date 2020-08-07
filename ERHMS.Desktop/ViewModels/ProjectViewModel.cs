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
using System.Windows.Data;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewItem : ObservableObject
        {
            public Epi.View View { get; }
            public string Title { get; }
            public int RecordCount { get; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
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

            public override int GetHashCode() => View.Id;

            public override bool Equals(object obj)
            {
                return obj is ViewItem viewItem && viewItem.View.Id == View.Id;
            }
        }

        public Project Project { get; }

        // TODO: Encapsulate as SelectableCollectionView<T> where T : ISelectable?
        private ICollection<ViewItem> viewItems;
        public ICollectionView ViewItems { get; }

        public Command RefreshCommand { get; }
        public Command CustomizeCommand { get; }
        public Command ViewDataCommand { get; }
        public Command EnterDataCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            viewItems = new List<ViewItem>();
            ViewItems = CollectionViewSource.GetDefaultView(viewItems);
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            CustomizeCommand = new SyncCommand(Customize, HasSelectedViewItem, ErrorBehavior.Raise);
            ViewDataCommand = new AsyncCommand(ViewDataAsync, HasSelectedViewItem, ErrorBehavior.Raise);
            EnterDataCommand = new SyncCommand(EnterData, HasSelectedViewItem, ErrorBehavior.Raise);
        }

        private void RefreshInternal()
        {
            viewItems.Clear();
            foreach (Epi.View view in Project.Views)
            {
                viewItems.Add(new ViewItem(view));
            }
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                RefreshInternal();
            });
            ViewItems.Refresh();
        }

        public bool HasSelectedViewItem()
        {
            return ViewItems.CurrentItem != null;
        }

        public void Customize()
        {
            // TODO
        }

        public async Task ViewDataAsync()
        {
            ViewViewModel content = null;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningViewTaskName);
            await progress.RunAsync(() =>
            {
                content = new ViewViewModel(this, ((ViewItem)ViewItems.CurrentItem).View);
            });
            MainViewModel.Current.Content = content;
        }

        public void EnterData()
        {
            // TODO
        }
    }
}
