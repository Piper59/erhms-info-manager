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
        public class RecordListChildViewModel : ListViewModel<RecordViewModel>
        {
            private ICollection<RecordViewModel> items;

            public RecordListChildViewModel(IEnumerable<RecordViewModel> items)
            {
                this.items = items.ToList();
                Refresh();
            }

            protected override IEnumerable<RecordViewModel> GetItems()
            {
                return items;
            }
        }

        private static IEnumerable<DataGridColumn> GetColumns(View view)
        {
            yield return new DataGridTextColumn
            {
                Header = "Remote",
                Binding = new Binding("Record.ModifiedOn"),
                IsReadOnly = true
            };
            yield return new DataGridTextColumn
            {
                Header = "Local",
                Binding = new Binding("Entity.ModifiedOn"),
                IsReadOnly = true
            };
            yield return new DataGridCheckBoxColumn
            {
                Header = "Import",
                Binding = new Binding("Import")
            };
            foreach (DataGridColumn column in DataGridExtensions.GetInputColumns(Context.Project, view, view.Id != Context.Responders.View.Id))
            {
                column.IsReadOnly = true;
                yield return column;
            }
        }

        public View View { get; private set; }
        public ICollection<DataGridColumn> Columns { get; private set; }
        public RecordListChildViewModel Records { get; private set; }

        public RecordListViewModel(View view, IEnumerable<RecordViewModel> records)
        {
            Title = "Import";
            View = view;
            Columns = GetColumns(view).ToList();
            Records = new RecordListChildViewModel(records);
        }

        public async Task PostpopulateAsync()
        {
            await TaskEx.WhenAll();
            // TODO
        }

        public async Task ImportAsync()
        {
            await TaskEx.WhenAll();
            // TODO
            //Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            //foreach (...)
            //{
            //    entities.Save(record);
            //}
            //ServiceLocator.Dialog.Notify(Resources.WebImported);
            //if (view.Id == Context.Responders.View.Id)
            //{
            //    ServiceLocator.Data.Refresh(typeof(Responder));
            //}
        }
    }
}
