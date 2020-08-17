using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public partial class ViewViewModel
    {
        public class RecordsChildViewModel : ObservableObject
        {
            public class Status : ObservableObject
            {
                public static readonly IReadOnlyCollection<Status> All = new Status[]
                {
                    new Status(RecordStatus.Undeleted, "Undeleted"),
                    new Status(RecordStatus.Deleted, "Deleted"),
                    new Status(null, "All")
                };

                public short? Value { get; }
                public string Text { get; }

                private Status(short? value, string text)
                {
                    Value = value;
                    Text = text;
                }
            }

            public class Item : ObservableObject, ISelectable
            {
                public Record Record { get; }

                private bool isSelected;
                public bool IsSelected
                {
                    get { return isSelected; }
                    set { SetProperty(ref isSelected, value); }
                }

                public Item(Record record)
                {
                    Record = record;
                }

                public override int GetHashCode()
                {
                    return Record.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    return obj is Item item && item.Record.Equals(Record);
                }
            }

            private readonly RecordRepository repository;

            public Epi.View View { get; }

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
                        SetFilter();
                    }
                }
            }

            public ICollectionView Statuses { get; }
            public IReadOnlyList<string> FieldNames { get; private set; }

            private readonly CustomCollectionView<Item> items;
            public ICustomCollectionView<Item> Items => items;

            public ICommand ClearSearchTextCommand { get; }
            public ICommand CreateCommand { get; }
            public ICommand EditCommand { get; }
            public ICommand DeleteCommand { get; }
            public ICommand UndeleteCommand { get; }

            public RecordsChildViewModel(Epi.View view)
            {
                View = view;
                repository = new RecordRepository(view);
                Statuses = new ListCollectionView(Status.All.ToList());
                Statuses.CurrentChanged += (sender, e) => SetFilter();
                items = new CustomCollectionView<Item>()
                {
                    PageSize = 100
                };
                RefreshDataInternal();
                SetFilter();
                ClearSearchTextCommand = new SyncCommand(ClearSearchText);
                CreateCommand = new AsyncCommand(CreateAsync);
                EditCommand = new AsyncCommand(EditAsync, items.HasSelectedItem);
                DeleteCommand = new AsyncCommand(DeleteAsync, items.HasSelectedItem);
                UndeleteCommand = new AsyncCommand(UndeleteAsync, items.HasSelectedItem);
            }

            private void RefreshDataInternal()
            {
                FieldNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
                items.Source.Clear();
                if (repository.TableExists())
                {
                    items.Source.AddRange(repository.Select().Select(record => new Item(record)));
                }
            }

            public void RefreshData()
            {
                View.LoadFields();
                RefreshDataInternal();
            }

            public void RefreshView()
            {
                OnPropertyChanged(nameof(FieldNames));
                items.Refresh();
            }

            private void SetFilter()
            {
                Predicate<Item> searchFilter = item => true;
                Predicate<Item> statusFilter = item => true;
                if (searchText != "")
                {
                    searchFilter = item =>
                    {
                        foreach (string fieldName in FieldNames)
                        {
                            object value = item.Record.Properties[fieldName];
                            if (value != null && value.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                }
                short? status = ((Status)Statuses.CurrentItem)?.Value;
                if (status != null)
                {
                    statusFilter = item => item.Record.Status == status;
                }
                items.TypedFilter = item => searchFilter(item) && statusFilter(item);
            }

            public void ClearSearchText()
            {
                SearchText = "";
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
                await MainViewModel.Current.StartEpiInfoAsync(
                    Module.Enter,
                    $"/project:{View.Project.FilePath}",
                    $"/view:{View.Name}",
                    $"/record:{items.SelectedItem.Record.UniqueKey}");
            }

            private async Task SetDeletedAsync(bool deleted)
            {
                string title = deleted ? ResX.DeletingRecordsTitle : ResX.UndeletingRecordsTitle;
                IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                progress.Title = title;
                progress.CanUserCancel = true;
                await progress.RunAsync(() =>
                {
                    foreach (Item item in items.SelectedItems)
                    {
                        if (progress.IsUserCancellationRequested)
                        {
                            break;
                        }
                        repository.SetDeleted(item.Record, deleted);
                    }
                });
                items.Refresh();
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
}
