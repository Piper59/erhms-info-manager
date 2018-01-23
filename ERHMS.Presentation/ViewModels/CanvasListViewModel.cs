using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
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

            public CanvasListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Canvas> GetItems()
            {
                IEnumerable<Canvas> canvases;
                if (Incident == null)
                {
                    canvases = Context.Canvases.SelectUndeleted();
                }
                else
                {
                    canvases = Context.Canvases.SelectByIncidentId(Incident.IncidentId);
                }
                return canvases.OrderBy(canvas => canvas.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(canvas => canvas.Incident?.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Canvas item)
            {
                yield return item.Name;
                yield return item.Incident?.Name;
            }
        }

        public CanvasListChildViewModel Canvases { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }

        public CanvasListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Dashboards";
            Canvases = new CanvasListChildViewModel(services, incident);
            OpenCommand = new AsyncCommand(OpenAsync, Canvases.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Canvases.HasSelectedItem);
            LinkCommand = new AsyncCommand(LinkAsync, Canvases.HasSelectedItem);
        }

        public async Task OpenAsync()
        {
            Canvas canvas = Context.Canvases.Refresh(Canvases.SelectedItem);
            await Services.Wrapper.InvokeAsync(AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected dashboard?", "Delete"))
            {
                Context.CanvasLinks.DeleteByCanvasId(Canvases.SelectedItem.CanvasId);
                Context.Project.DeleteCanvas(Canvases.SelectedItem.CanvasId);
                Services.Data.Refresh(typeof(Canvas));
            }
        }

        public async Task LinkAsync()
        {
            using (CanvasLinkViewModel model = new CanvasLinkViewModel(Services, Context.Canvases.Refresh(Canvases.SelectedItem)))
            {
                await Services.Dialog.ShowAsync(model);
            }
        }

        public override void Dispose()
        {
            Canvases.Dispose();
            base.Dispose();
        }
    }
}
