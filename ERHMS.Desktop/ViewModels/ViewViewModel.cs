using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public ProjectViewModel Parent { get; }
        public Epi.View View { get; }
        public IReadOnlyList<string> FieldNames { get; private set; }

        private readonly SelectableListCollectionView<RecordItem> recordItems;
        public ICollectionView RecordItems => recordItems;

        public Command RefreshCommand { get; }
        public Command GoBackCommand { get; }
        public Command CreateCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command UndeleteCommand { get; }

        public ViewViewModel(ProjectViewModel parent, Epi.View view)
        {
            Parent = parent;
            View = view;
            repository = new RecordRepository(view);
            recordItems = new SelectableListCollectionView<RecordItem>(new List<RecordItem>());
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            GoBackCommand = new AsyncCommand(GoBackAsync, CanGoBack, ErrorBehavior.Raise);
            CreateCommand = new SimpleAsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            DeleteCommand = new AsyncCommand(DeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
        }

        public ViewViewModel(Epi.View view)
            : this(null, view) { }

        private void RefreshInternal()
        {
            FieldNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
            recordItems.Source.Clear();
            if (repository.TableExists())
            {
                foreach (Record record in repository.Select())
                {
                    recordItems.Source.Add(new RecordItem(record));
                }
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

        public bool CanGoBack()
        {
            return Parent != null;
        }

        public async Task GoBackAsync()
        {
            await Parent.RefreshAsync();
            MainViewModel.Current.Content = Parent;
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
