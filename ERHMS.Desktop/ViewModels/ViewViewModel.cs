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
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ObservableObject
    {
        public class RecordItem : ObservableObject
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

        // TODO: Encapsulate as SelectableCollectionView<T> where T : ISelectable?
        private ICollection<RecordItem> recordItems;
        public ICollectionView RecordItems { get; }

        public Command RefreshCommand { get; }
        public Command GoBackCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command UndeleteCommand { get; }

        public ViewViewModel(ProjectViewModel parent, Epi.View view)
        {
            Parent = parent;
            View = view;
            repository = new RecordRepository(view);
            recordItems = new List<RecordItem>();
            RecordItems = CollectionViewSource.GetDefaultView(recordItems);
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            GoBackCommand = new AsyncCommand(GoBackAsync, CanGoBack, ErrorBehavior.Raise);
            EditCommand = new SyncCommand(Edit, HasSelectedRecordItem, ErrorBehavior.Raise);
            DeleteCommand = new SyncCommand(Delete, HasSelectedRecordItem, ErrorBehavior.Raise);
            UndeleteCommand = new SyncCommand(Undelete, HasSelectedRecordItem, ErrorBehavior.Raise);
        }

        public ViewViewModel(Epi.View view)
            : this(null, view) { }

        private void RefreshInternal()
        {
            FieldNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
            recordItems.Clear();
            if (repository.TableExists())
            {
                foreach (Record record in repository.Select())
                {
                    recordItems.Add(new RecordItem(record));
                }
            }
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingViewTaskName);
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

        public bool HasSelectedRecordItem()
        {
            return RecordItems.CurrentItem != null;
        }

        public void Edit()
        {
            // TODO
        }

        public void Delete()
        {
            // TODO
        }

        public void Undelete()
        {
            // TODO
        }
    }
}
