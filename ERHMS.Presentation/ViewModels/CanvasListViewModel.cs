using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasListViewModel : ListViewModel<Canvas>
    {
        public Incident Incident { get; private set; }

        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get { return openCommand ?? (openCommand = new RelayCommand(Open, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        private RelayCommand linkCommand;
        public ICommand LinkCommand
        {
            get { return linkCommand ?? (linkCommand = new RelayCommand(Link, HasSelectedItem)); }
        }

        public CanvasListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Dashboards";
            Incident = incident;
            SelectionChanged += (sender, e) =>
            {
                openCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                linkCommand.RaiseCanExecuteChanged();
            };
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
            return canvases.OrderBy(canvas => canvas.Name).ThenBy(canvas => canvas.Incident?.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Canvas item)
        {
            yield return item.Name;
            yield return item.Incident?.Name;
        }

        public void Open()
        {
            Canvas canvas = Context.Canvases.SelectById(SelectedItem.CanvasId);
            AnalysisDashboard.OpenCanvas.Create(Context.Project.FilePath, canvas.CanvasId, canvas.Content).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected dashboard?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Context.CanvasLinks.DeleteByCanvasId(SelectedItem.CanvasId);
                Context.Project.DeleteCanvas(SelectedItem.CanvasId);
                MessengerInstance.Send(new RefreshMessage(typeof(Canvas)));
            };
            MessengerInstance.Send(msg);
        }

        public void Link()
        {
            Dialogs.ShowAsync(new CanvasLinkViewModel(Services, SelectedItem));
        }
    }
}
