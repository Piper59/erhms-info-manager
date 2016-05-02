using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
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
    public class PgmListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<Pgm, string>> FilterPropertyAccessors = new Func<Pgm, string>[]
        {
            pgm => pgm.Name,
            pgm => pgm.Comment,
            pgm => pgm.Author
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
                Pgms.Refresh();
            }
        }

        private ICollectionView pgms;
        public ICollectionView Pgms
        {
            get { return pgms; }
            set { Set(() => Pgms, ref pgms, value); }
        }

        private Pgm selectedPgm;
        public Pgm SelectedPgm
        {
            get
            {
                return selectedPgm;
            }
            set
            {
                if (!Set(() => SelectedPgm, ref selectedPgm, value))
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
            Refresh();
            OpenCommand = new RelayCommand(Open, HasSelectedPgm);
            DeleteCommand = new RelayCommand(Delete, HasSelectedPgm);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Pgm>>(this, OnRefreshMessage);
        }

        public bool HasSelectedPgm()
        {
            return SelectedPgm != null;
        }

        public void Open()
        {
            Analysis.OpenPgm(SelectedPgm);
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
                PgmLink pgmLink = DataContext.PgmLinks.SelectByPgmId(SelectedPgm.PgmId);
                if (pgmLink != null)
                {
                    DataContext.PgmLinks.Delete(pgmLink);
                }
                DataContext.Project.DeletePgm(SelectedPgm.PgmId);
                Messenger.Default.Send(new RefreshMessage<Pgm>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void Refresh()
        {
            Pgms = CollectionViewSource.GetDefaultView(DataContext.GetLinkedPgms(IncidentId).OrderBy(pgm => pgm.Name));
            Pgms.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Pgm pgm = (Pgm)item;
            foreach (Func<Pgm, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(pgm);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
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
