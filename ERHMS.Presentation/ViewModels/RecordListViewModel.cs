using Epi;
using Epi.Fields;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Wrappers;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class RecordListViewModel : ListViewModelBase<ViewEntity>
    {
        public View View { get; private set; }
        public ViewEntityRepository<ViewEntity> Entities { get; private set; }

        private ICollection<DataGridColumn> columns;
        public ICollection<DataGridColumn> Columns
        {
            get { return columns; }
            private set { Set(nameof(Columns), ref columns, value); }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand UndeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public RecordListViewModel(View view)
        {
            Title = view.Name;
            View = view;
            DataContext.Project.CollectedData.EnsureDataTablesExist(view);
            Entities = new ViewEntityRepository<ViewEntity>(DataContext.Driver, view);
            List<int> fieldIds = DataContext.Project.GetSortedFieldIds(view.Id).ToList();
            Columns = view.Fields.TableColumnFields
                .Cast<Field>()
                .OrderBy(field => fieldIds.IndexOf(field.Id))
                .Select(field => (DataGridColumn)new DataGridTextColumn
                {
                    Header = field.Name,
                    Binding = new Binding(field.Name)
                })
                .ToList();
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasAnySelectedItems);
            UndeleteCommand = new RelayCommand(Undelete, HasAnySelectedItems);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                UndeleteCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<ViewEntity> GetItems()
        {
            return Entities.Select();
        }

        private void Invoke(Wrapper wrapper)
        {
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.RecordSaved)
                {
                    Refresh();
                }
            };
            wrapper.Invoke();
        }

        public void Create()
        {
            Invoke(Enter.OpenNewRecord.Create(DataContext.Project.FilePath, View.Name));
        }

        public void Edit()
        {
            Invoke(Enter.OpenRecord.Create(DataContext.Project.FilePath, View.Name, SelectedItem.UniqueKey.Value));
        }

        public void Delete()
        {
            foreach (ViewEntity entity in SelectedItems)
            {
                if (!entity.Deleted)
                {
                    Entities.Delete(entity);
                }
            }
        }

        public void Undelete()
        {
            foreach (ViewEntity entity in SelectedItems)
            {
                if (entity.Deleted)
                {
                    Entities.Undelete(entity);
                }
            }
        }
    }
}
