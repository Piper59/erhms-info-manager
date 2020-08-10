using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ObservableObject
    {
        public class RecordItem : ObservableObject, ISelectable
        {
            public Record Record { get; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            public RecordItem(Record record)
            {
                Record = record;
            }

            public override int GetHashCode() => Record.GetHashCode();

            public override bool Equals(object obj)
            {
                return obj is RecordItem recordItem && recordItem.Record.Equals(Record);
            }
        }

        private readonly RecordRepository repository;

        public Project Project { get; }
        public Epi.View View { get; }
        public IReadOnlyList<string> FieldNames { get; private set; }

        private readonly CustomCollectionView<RecordItem> recordItems;
        public ICustomCollectionView<RecordItem> RecordItems => recordItems;

        public ICommand RefreshCommand { get; }
        public ICommand GoUpCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndeleteCommand { get; }

        public ViewViewModel(Project project, Epi.View view)
        {
            Project = project;
            View = view;
            repository = new RecordRepository(view);
            recordItems = new CustomCollectionView<RecordItem>(new List<RecordItem>());
            // TODO: Remove
            RefreshInternal();
            RecordItems.Refresh();
            RefreshCommand = new AsyncCommand(RefreshAsync, Command.Always, ErrorBehavior.Raise);
            GoUpCommand = new AsyncCommand(GoUpAsync, Command.Always, ErrorBehavior.Raise);
            CreateCommand = new AsyncCommand(CreateAsync, Command.Always, ErrorBehavior.Raise);
            EditCommand = new AsyncCommand(EditAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            DeleteCommand = new AsyncCommand(DeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
        }

        private void RefreshInternal()
        {
            FieldNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
            recordItems.Source.Clear();
            if (repository.TableExists())
            {
                recordItems.Source.AddRange(repository.Select().Select(record => new RecordItem(record)));
            }
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingViewTaskName, false);
            await progress.RunAsync(() =>
            {
                View.LoadFields();
                RefreshInternal();
            });
            OnPropertyChanged(nameof(FieldNames));
            RecordItems.Refresh();
        }

        public async Task GoUpAsync()
        {
            ProjectViewModel content = null;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName, false);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                content = new ProjectViewModel(Project);
            });
            MainViewModel.Current.Content = content;
        }

        public async Task CreateAsync()
        {
            await MainViewModel.Current.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{View.Project.FilePath}",
                $"/view:{View.Name}",
                "/record:*");
        }

        public async Task EditAsync()
        {
            Record record = recordItems.SelectedItem.Record;
            await MainViewModel.Current.StartEpiInfoAsync(
                Module.Enter,
                $"/project:{View.Project.FilePath}",
                $"/view:{View.Name}",
                $"/record:{record.UniqueKey}");
        }

        private async Task SetDeletedAsync(bool deleted)
        {
            string taskName = deleted ? Resources.DeletingRecordsTaskName : Resources.UndeletingRecordsTaskName;
            IProgressService progress = ServiceProvider.GetProgressService(taskName, true);
            await progress.RunAsync(() =>
            {
                foreach (RecordItem recordItem in recordItems.SelectedItems)
                {
                    if (progress.IsUserCancellationRequested)
                    {
                        break;
                    }
                    repository.SetDeleted(recordItem.Record, deleted);
                }
            });
        }

        public async Task DeleteAsync()
        {
            await SetDeletedAsync(true);
        }

        public async Task UndeleteAsync()
        {
            await SetDeletedAsync(false);
        }
    }
}
