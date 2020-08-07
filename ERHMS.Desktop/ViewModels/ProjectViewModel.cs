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
        public class ViewItem : ObservableObject, ISelectable
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

        private readonly SelectableListCollectionView<ViewItem> viewItems;
        public ICollectionView ViewItems => viewItems;

        public Command RefreshCommand { get; }
        public Command CustomizeCommand { get; }
        public Command ViewDataCommand { get; }
        public Command EnterDataCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            viewItems = new SelectableListCollectionView<ViewItem>(new List<ViewItem>());
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            CustomizeCommand = new SyncCommand(Customize, viewItems.HasSelectedItem, ErrorBehavior.Raise);
            ViewDataCommand = new AsyncCommand(ViewDataAsync, viewItems.HasSelectedItem, ErrorBehavior.Raise);
            EnterDataCommand = new SyncCommand(EnterData, viewItems.HasSelectedItem, ErrorBehavior.Raise);
        }

        private void RefreshInternal()
        {
            viewItems.Source.Clear();
            foreach (Epi.View view in Project.Views)
            {
                viewItems.Source.Add(new ViewItem(view));
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
                content = new ViewViewModel(this, viewItems.SelectedItem.View);
            });
            MainViewModel.Current.Content = content;
        }

        public void EnterData()
        {
            // TODO
        }
    }
}
