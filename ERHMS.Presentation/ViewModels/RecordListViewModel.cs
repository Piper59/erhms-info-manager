using Epi;
using Epi.Fields;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class RecordListViewModel : DocumentViewModel
    {
        public class RecordListChildViewModel : ListViewModel<ViewEntity>
        {
            public ViewEntityRepository<ViewEntity> Entities { get; private set; }

            public RecordListChildViewModel(View view)
            {
                Entities = new ViewEntityRepository<ViewEntity>(Context.Database, view);
                Refresh();
            }

            protected override IEnumerable<ViewEntity> GetItems()
            {
                return Entities.SelectOrdered();
            }
        }

        public View View { get; private set; }
        public ICollection<DataGridColumn> Columns { get; private set; }
        public RecordListChildViewModel Records { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand PostpopulateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand UndeleteCommand { get; private set; }

        public RecordListViewModel(View view)
        {
            Title = view.Name;
            View = view;
            List<int> fieldIds = Context.Project.GetSortedFieldIds(view.Id).ToList();
            Columns = view.Fields.DataFields
                .Cast<Field>()
                .OrderBy(field => field.Name == ColumnNames.REC_STATUS ? int.MaxValue : fieldIds.IndexOf(field.Id))
                .Select(field => (DataGridColumn)new DataGridTextColumn
                {
                    Header = field.Name,
                    Binding = new Binding(field.Name)
                })
                .ToList();
            Records = new RecordListChildViewModel(view);
            CreateCommand = new AsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, Records.HasOneSelectedItem);
            PostpopulateCommand = new AsyncCommand(PostpopulateAsync, CanPostpopulate);
            DeleteCommand = new Command(Delete, Records.HasAnySelectedItems);
            UndeleteCommand = new Command(Undelete, Records.HasAnySelectedItems);
        }

        public async Task CreateAsync()
        {
            Domain.View view = Context.Views.SelectById(View.Id);
            if (view.HasResponderIdField)
            {
                PrepopulateViewModel model = new PrepopulateViewModel(view);
                await ServiceLocator.Dialog.ShowAsync(model);
            }
            else
            {
                Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name);
                wrapper.AddRecordSavedHandler();
                await ServiceLocator.Wrapper.InvokeAsync(wrapper);
            }
        }

        public async Task EditAsync()
        {
            Wrapper wrapper = Enter.OpenRecord.Create(Context.Project.FilePath, View.Name, Records.SelectedItems.First().UniqueKey.Value);
            wrapper.AddRecordSavedHandler();
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public bool CanPostpopulate()
        {
            return View.Fields.Contains(FieldNames.ResponderId) && Records.HasOneSelectedItem();
        }

        public async Task PostpopulateAsync()
        {
            ViewEntity entity = Records.Entities.Refresh(Records.SelectedItems.First());
            PostpopulateViewModel model = new PostpopulateViewModel(View, entity);
            model.Saved += (sender, e) =>
            {
                Records.Refresh();
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        private void SetDeleted(bool deleted)
        {
            using (ServiceLocator.Busy.Begin())
            {
                foreach (ViewEntity entity in Records.Entities.Refresh(Records.SelectedItems))
                {
                    if (entity.Deleted != deleted)
                    {
                        entity.Deleted = deleted;
                        Records.Entities.Save(entity);
                    }
                }
            }
            Records.Refresh();
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
