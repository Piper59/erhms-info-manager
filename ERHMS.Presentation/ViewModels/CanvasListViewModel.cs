using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<Canvas, string>> FilterPropertyAccessors = new Func<Canvas, string>[]
        {
            canvas => canvas.Name
        };

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (!Set(() => Filter, ref filter, value))
                {
                    return;
                }
                Canvases.Refresh();
            }
        }

        private ICollectionView canvases;
        public ICollectionView Canvases
        {
            get { return canvases; }
            set { Set(() => Canvases, ref canvases, value); }
        }

        private Canvas selectedCanvas;
        public Canvas SelectedCanvas
        {
            get
            {
                return selectedCanvas;
            }
            set
            {
                if (!Set(() => SelectedCanvas, ref selectedCanvas, value))
                {
                    return;
                }
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public CanvasListViewModel(Incident incident)
        {
            if (incident == null)
            {
                Title = "Dashboards";
            }
            else
            {
                Title = string.Format("{0} Dashboards", incident.Name);
            }
            Incident = incident;
            Refresh();
            OpenCommand = new RelayCommand(Open, HasSelectedCanvas);
            DeleteCommand = new RelayCommand(Delete, HasSelectedCanvas);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Canvas>>(this, OnRefreshMessage);
        }

        public bool HasSelectedCanvas()
        {
            return SelectedCanvas != null;
        }

        public void Open()
        {
            AnalysisDashboard.OpenCanvas(DataContext.Project, SelectedCanvas, IncidentId);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this dashboard?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                CanvasLink canvasLink = DataContext.CanvasLinks.SelectByCanvasId(SelectedCanvas.CanvasId);
                if (canvasLink != null)
                {
                    DataContext.CanvasLinks.Delete(canvasLink);
                }
                DataContext.Project.DeleteCanvas(SelectedCanvas.CanvasId);
                Messenger.Default.Send(new RefreshMessage<Canvas>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void Refresh()
        {
            Canvases = CollectionViewSource.GetDefaultView(DataContext.GetLinkedCanvases(IncidentId).OrderBy(canvas => canvas.Name));
            Canvases.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Canvas canvas = (Canvas)item;
            foreach (Func<Canvas, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(canvas);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<Canvas> msg)
        {
            if (msg.IncidentId == IncidentId)
            {
                Refresh();
            }
        }
    }
}
