using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
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
    public class PgmListViewModel : ListViewModelBase<Pgm>
    {
        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public PgmListViewModel(Incident incident)
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
            Messenger.Default.Register<RefreshListMessage<Pgm>>(this, OnRefreshPgmListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitleWithIncidentName("Analyses", Incident);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<Pgm> pgms;
            if (Incident == null)
            {
                pgms = DataContext.GetUnlinkedPgms();
            }
            else
            {
                pgms = DataContext.GetLinkedPgms(IncidentId);
            }
            return CollectionViewSource.GetDefaultView(pgms.OrderBy(pgm => pgm.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Pgm item)
        {
            yield return item.Name;
            yield return item.Comment;
            yield return item.Author;
        }

        public void Open()
        {
            Analysis.OpenPgm(DataContext.Project.GetPgmById(SelectedItem.PgmId), false);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected analysis?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.PgmLinks.DeleteByPgmId(SelectedItem.PgmId);
                DataContext.Project.DeletePgm(SelectedItem);
                Messenger.Default.Send(new RefreshListMessage<Pgm>(IncidentId));
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

        private void OnRefreshPgmListMessage(RefreshListMessage<Pgm> msg)
        {
            if (StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
