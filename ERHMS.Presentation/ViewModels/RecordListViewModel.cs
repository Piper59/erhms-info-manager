using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Wrappers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class RecordListViewModel : ListViewModel<ViewEntity>
    {
        public ViewEntityRepository<ViewEntity> Entities { get; private set; }

        private ICollection<DataGridColumn> columns;
        public ICollection<DataGridColumn> Columns
        {
            get { return columns; }
            private set { Set(nameof(Columns), ref columns, value); }
        }

        protected override IEnumerable<Type> RefreshTypes
        {
            get { yield break; }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand UndeleteCommand { get; private set; }

        public RecordListViewModel(IServiceManager services, View view)
            : base(services)
        {
            Title = view.Name;
            Entities = new ViewEntityRepository<ViewEntity>(Context, view);
            List<int> fieldIds = Context.Project.GetSortedFieldIds(view.Id).ToList();
            Columns = view.Fields.DataFields
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
            EditCommand = new RelayCommand(Edit, HasSingleSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            UndeleteCommand = new RelayCommand(Undelete, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                UndeleteCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<ViewEntity> GetItems()
        {
            return Entities.SelectOrdered();
        }

        public void Create()
        {
            Enter.OpenNewRecord.Create(Context.Project.FilePath, Entities.View.Name).Invoke();
        }

        public void Edit()
        {
            Enter.OpenRecord.Create(Context.Project.FilePath, Entities.View.Name, SelectedItem.UniqueKey.Value).Invoke();
        }

        private void SetDeleted(bool deleted)
        {
            foreach (ViewEntity entity in SelectedItems)
            {
                if (entity.Deleted != deleted)
                {
                    entity.Deleted = deleted;
                    Entities.Save(entity);
                }
            }
        }

        public void Delete()
        {
            SetDeleted(true);
        }

        public void Undelete()
        {
            SetDeleted(false);
        }
    }
}
