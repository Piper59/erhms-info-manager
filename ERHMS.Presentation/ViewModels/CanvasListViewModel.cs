using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class CanvasListViewModel : ListViewModelBase<Canvas>
    {
        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
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
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<Canvas>>(this, OnRefreshListMessage);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<Canvas> canvases;
            if (Incident == null)
            {
                canvases = DataContext.GetUnlinkedCanvases();
            }
            else
            {
                canvases = DataContext.GetLinkedCanvases(IncidentId);
            }
            return CollectionViewSource.GetDefaultView(canvases.OrderBy(canvas => canvas.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Canvas item)
        {
            yield return item.Name;
        }

        public void Open()
        {
            AnalysisDashboard.OpenCanvas(DataContext.Project, DataContext.Project.GetCanvasById(SelectedItem.CanvasId), IncidentId);
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
                CanvasLink canvasLink = DataContext.CanvasLinks.SelectByCanvasId(SelectedItem.CanvasId);
                if (canvasLink != null)
                {
                    DataContext.CanvasLinks.Delete(canvasLink);
                }
                DataContext.Project.DeleteCanvas(SelectedItem.CanvasId);
                Messenger.Default.Send(new RefreshListMessage<Canvas>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshListMessage(RefreshListMessage<Canvas> msg)
        {
            if (msg.IncidentId == IncidentId)
            {
                Refresh();
            }
        }
    }
}
