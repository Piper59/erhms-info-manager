using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasListViewModel : DocumentViewModel
    {
        public class CanvasListChildViewModel : ListViewModel<Canvas>
        {
            public Incident Incident { get; private set; }

            public CanvasListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Canvas> GetItems()
            {
                IEnumerable<Canvas> canvases = Incident == null
                    ? Context.Canvases.SelectUndeleted()
                    : Context.Canvases.SelectByIncidentId(Incident.IncidentId);
                return canvases.OrderBy(canvas => canvas.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(canvas => canvas.Incident?.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Canvas item)
            {
                yield return item.Name;
                yield return item.Incident?.Name;
            }
        }

        public Incident Incident { get; private set; }
        public CanvasListChildViewModel Canvases { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }

        public CanvasListViewModel(Incident incident)
        {
            Title = "Dashboards";
            Incident = incident;
            Canvases = new CanvasListChildViewModel(incident);
            OpenCommand = new AsyncCommand(OpenAsync, Canvases.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Canvases.HasSelectedItem);
            LinkCommand = new AsyncCommand(LinkAsync, Canvases.HasSelectedItem);
        }

        public async Task OpenAsync()
        {
            Canvas canvas = Context.Canvases.Refresh(Canvases.SelectedItem);
            Wrapper wrapper = AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.CanvasConfirmDelete, "Delete"))
            {
                Context.CanvasLinks.DeleteByCanvasId(Canvases.SelectedItem.CanvasId);
                Context.Project.DeleteCanvas(Canvases.SelectedItem.CanvasId);
                ServiceLocator.Data.Refresh(typeof(Canvas));
            }
        }

        public async Task LinkAsync()
        {
            CanvasLinkViewModel model = new CanvasLinkViewModel(Context.Canvases.Refresh(Canvases.SelectedItem));
            await ServiceLocator.Dialog.ShowAsync(model);
        }
    }
}
