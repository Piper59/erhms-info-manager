using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
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
                Binding = new Binding("Entity.ModifiedOn")
                {
                    FallbackValue = "Not found"
                },
                IsReadOnly = true
            };
            // TODO: Allow single-click to change state
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

        public ICommand PostpopulateCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }

        public RecordListViewModel(View view, IEnumerable<RecordViewModel> records)
        {
            Title = "Import";
            View = view;
            Columns = GetColumns(view).ToList();
            Records = new RecordListChildViewModel(records);
        }

        public bool CanPostpopulate()
        {
            return View.Fields.Contains(FieldNames.ResponderId) && Records.HasOneSelectedItem();
        }

        public async Task PostpopulateAsync()
        {
            PostpopulateViewModel model = new PostpopulateViewModel(Records.SelectedItem.Responder?.ResponderId);
            model.Saved += (sender, e) =>
            {
                Records.SelectedItem.Responder = e.ResponderId == null ? null : Context.Responders.SelectById(e.ResponderId);
                Records.SelectedItem.Record[FieldNames.ResponderId] = e.ResponderId;
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task ImportAsync()
        {
            await ServiceLocator.Dialog.BlockAsync(Resources.WebImporting, () =>
            {
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(Context.Database, View);
                foreach (RecordViewModel record in Records.Items.Where(record => record.Import))
                {
                    entities.Save(record.Record);
                }
            });
            ServiceLocator.Dialog.Notify(Resources.WebImported);
            if (View.Id == Context.Responders.View.Id)
            {
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }
    }
}
