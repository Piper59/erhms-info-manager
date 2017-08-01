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
using System.Windows.Input;

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

        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create)); }
        }

        private RelayCommand editCommand;
        public ICommand EditCommand
        {
            get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSingleSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        private RelayCommand undeleteCommand;
        public ICommand UndeleteCommand
        {
            get { return undeleteCommand ?? (undeleteCommand = new RelayCommand(Undelete, HasSelectedItem)); }
        }

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
            SelectionChanged += (sender, e) =>
            {
                editCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                undeleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
        }

        protected override IEnumerable<ViewEntity> GetItems()
        {
            return Entities.Select();
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
                if (!entity.Deleted)
                {
                    entity.Deleted = deleted;
                    Entities.Save(entity);
                }
            }
            Refresh();
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
