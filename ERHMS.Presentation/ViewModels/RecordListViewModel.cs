using Epi;
using Epi.Fields;
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

            public RecordListChildViewModel(IServiceManager services, View view)
                : base(services)
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

        public RecordListViewModel(IServiceManager services, View view)
            : base(services)
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
            Records = new RecordListChildViewModel(services, view);
            CreateCommand = new AsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, Records.HasOneSelectedItem);
            PostpopulateCommand = new AsyncCommand(PostpopulateAsync, CanPostpopulate);
            DeleteCommand = new Command(Delete, Records.HasAnySelectedItems);
            UndeleteCommand = new Command(Undelete, Records.HasAnySelectedItems);
        }

        public async Task CreateAsync()
        {
            Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name);
            wrapper.AddRecordSavedHandler(Services);
            await Services.Wrapper.InvokeAsync(wrapper);
        }

        public async Task EditAsync()
        {
            Wrapper wrapper = Enter.OpenRecord.Create(Context.Project.FilePath, View.Name, Records.SelectedItems.First().UniqueKey.Value);
            wrapper.AddRecordSavedHandler(Services);
            await Services.Wrapper.InvokeAsync(wrapper);
        }

        public bool CanPostpopulate()
        {
            return View.Fields.Contains("ResponderID") && Records.HasOneSelectedItem();
        }

        public async Task PostpopulateAsync()
        {
            ViewEntity entity = Records.Entities.Refresh(Records.SelectedItems.First());
            using (PostpopulateViewModel model = new PostpopulateViewModel(Services, View, entity))
            {
                model.Saved += (sender, e) =>
                {
                    Records.Refresh();
                };
                await Services.Dialog.ShowAsync(model);
            }
        }

        private void SetDeleted(bool deleted)
        {
            foreach (ViewEntity entity in Records.Entities.Refresh(Records.SelectedItems))
            {
                if (entity.Deleted != deleted)
                {
                    entity.Deleted = deleted;
                    Records.Entities.Save(entity);
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

        public override void Dispose()
        {
            Records.Dispose();
            base.Dispose();
        }
    }
}
