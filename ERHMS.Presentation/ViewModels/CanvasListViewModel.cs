using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasListViewModel : ListViewModelBase<Link<Canvas>>
    {
        public class LinkInternalViewModel : LinkViewModelBase
        {
            public Link<Canvas> Canvas { get; private set; }

            public void Reset(Link<Canvas> canvas)
            {
                Reset(canvas.IncidentId);
                Canvas = canvas;
            }

            public override void Link()
            {
                DataContext.CanvasLinks.DeleteByCanvasId(Canvas.Data.CanvasId);
                CanvasLink canvasLink = DataContext.CanvasLinks.Create();
                canvasLink.CanvasId = Canvas.Data.CanvasId;
                canvasLink.IncidentId = SelectedIncidentId;
                DataContext.CanvasLinks.Save(canvasLink);
                Messenger.Default.Send(new RefreshListMessage<Canvas>(SelectedIncidentId));
                Active = false;
            }

            public override void Unlink()
            {
                DataContext.CanvasLinks.DeleteByCanvasId(Canvas.Data.CanvasId);
                Messenger.Default.Send(new RefreshListMessage<Canvas>(SelectedIncidentId));
                Active = false;
            }
        }

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        public LinkInternalViewModel LinkModel { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand IncidentCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public CanvasListViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            Refresh();
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                IncidentCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            LinkModel = new LinkInternalViewModel();
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            IncidentCommand = new RelayCommand(Link, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<Canvas>>(this, OnRefreshCanvasListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitle("Dashboards", Incident);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<Link<Canvas>> canvases;
            if (Incident == null)
            {
                canvases = DataContext.GetLinkedCanvases().Where(canvas => canvas.Incident == null || !canvas.Incident.Deleted);
            }
            else
            {
                canvases = DataContext.GetLinkedCanvases(IncidentId).Select(canvas => new Link<Canvas>(canvas, Incident));
            }
            return CollectionViewSource.GetDefaultView(canvases.OrderBy(canvas => canvas.Data.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Link<Canvas> item)
        {
            yield return item.Data.Name;
            yield return item.IncidentName;
        }

        public void Open()
        {
            AnalysisDashboard.OpenCanvas(DataContext.Project, DataContext.Project.GetCanvasById(SelectedItem.Data.CanvasId), SelectedItem.IncidentId).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage("Delete?", "Delete the selected dashboard?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.CanvasLinks.DeleteByCanvasId(SelectedItem.Data.CanvasId);
                DataContext.Project.DeleteCanvas(SelectedItem.Data);
                Messenger.Default.Send(new RefreshListMessage<Canvas>(SelectedItem.IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void Link()
        {
            LinkModel.Reset(SelectedItem);
            LinkModel.Active = true;
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshCanvasListMessage(RefreshListMessage<Canvas> msg)
        {
            if (Incident == null || StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
