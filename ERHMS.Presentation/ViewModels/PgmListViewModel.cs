using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
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
            if (incident == null)
            {
                Title = "Analyses";
            }
            else
            {
                Title = string.Format("{0} Analyses", incident.Name);
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
            Messenger.Default.Register<RefreshMessage<Pgm>>(this, OnRefreshMessage);
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
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this analysis?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                PgmLink pgmLink = DataContext.PgmLinks.SelectByPgmId(SelectedItem.PgmId);
                if (pgmLink != null)
                {
                    DataContext.PgmLinks.Delete(pgmLink);
                }
                DataContext.Project.DeletePgm(SelectedItem.PgmId);
                Messenger.Default.Send(new RefreshMessage<Pgm>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshMessage(RefreshMessage<Pgm> msg)
        {
            if (msg.IncidentId == IncidentId)
            {
                Refresh();
            }
        }
    }
}
