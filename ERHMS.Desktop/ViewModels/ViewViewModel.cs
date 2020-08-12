using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ObservableObject
    {
        public class RecordStatusItem : ObservableObject, ISelectable
        {
            public static readonly IReadOnlyCollection<RecordStatusItem> All = new RecordStatusItem[]
            {
                new RecordStatusItem(EpiInfo.Data.RecordStatus.Undeleted, "Undeleted"),
                new RecordStatusItem(EpiInfo.Data.RecordStatus.Deleted, "Deleted"),
                new RecordStatusItem(null, "All")
            };

            public short? RecordStatus { get; }
            public string DisplayText { get; }

            private bool isSelected;
            public bool IsSelected
            {
                get { return isSelected; }
                set { SetProperty(ref isSelected, value); }
            }

            private RecordStatusItem(short? recordStatus, string displayText)
            {
                RecordStatus = recordStatus;
                DisplayText = displayText;
            }
        }

        public class RecordItem : ObservableObject, ISelectable
        {
            public Record Record { get; }

            private bool isSelected;
            public bool IsSelected
            {
                get { return isSelected; }
                set { SetProperty(ref isSelected, value); }
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

        private string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    SetRecordItemFilter();
                }
            }
        }

        private readonly CustomCollectionView<RecordStatusItem> recordStatusItems;
        public ICustomCollectionView<RecordStatusItem> RecordStatusItems => recordStatusItems;

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
            recordStatusItems = new CustomCollectionView<RecordStatusItem>();
            recordStatusItems.Source.AddRange(RecordStatusItem.All);
            recordStatusItems.Refresh();
            recordStatusItems.MoveCurrentToPosition(0);
            recordStatusItems.CurrentChanged += (sender, e) => SetRecordItemFilter();
            recordItems = new CustomCollectionView<RecordItem>
            {
                PageSize = 100
            };
            RefreshInternal();
            SetRecordItemFilter();
            RefreshCommand = new AsyncCommand(RefreshAsync, Command.Always, ErrorBehavior.Raise);
            GoUpCommand = new AsyncCommand(GoUpAsync, Command.Always, ErrorBehavior.Raise);
            CreateCommand = new AsyncCommand(CreateAsync, Command.Always, ErrorBehavior.Raise);
            EditCommand = new AsyncCommand(EditAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            DeleteCommand = new AsyncCommand(DeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
            UndeleteCommand = new AsyncCommand(UndeleteAsync, recordItems.HasSelectedItem, ErrorBehavior.Raise);
        }

        private void SetRecordItemFilter()
        {
            Predicate<RecordItem> searchFilter = recordItem => true;
            Predicate<RecordItem> recordStatusFilter = recordItem => true;
            if (searchText != "")
            {
                searchFilter = recordItem =>
                {
                    foreach (string fieldName in FieldNames)
                    {
                        object value = recordItem.Record.Properties[fieldName];
                        if (value != null && value.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
            short? recordStatus = recordStatusItems.SelectedItem?.RecordStatus;
            if (recordStatus != null)
            {
                recordStatusFilter = recordItem => recordItem.Record.RecordStatus == recordStatus;
            }
            recordItems.TypedFilter = recordItem => searchFilter(recordItem) && recordStatusFilter(recordItem);
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
            recordItems.Refresh();
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
            recordItems.Refresh();
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
