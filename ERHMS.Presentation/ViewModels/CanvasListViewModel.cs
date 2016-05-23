using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.AnalysisDashboard;
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
            Incident = incident;
            UpdateTitle();
            Refresh();
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
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
            ConfirmMessage msg = new ConfirmMessage("Delete?", "Delete the selected dashboard?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.CanvasLinks.DeleteByCanvasId(SelectedItem.CanvasId);
                DataContext.Project.DeleteCanvas(SelectedItem);
                Messenger.Default.Send(new RefreshListMessage<Canvas>(IncidentId));
            };
            Messenger.Default.Send(msg);
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
            if (StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
