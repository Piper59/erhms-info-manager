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
    public class PgmListViewModel : ListViewModelBase<Link<Pgm>>
    {
        public class LinkInternalViewModel : LinkViewModelBase
        {
            public Link<Pgm> Pgm { get; private set; }

            public void Reset(Link<Pgm> pgm)
            {
                Reset(pgm.IncidentId);
                Pgm = pgm;
            }

            public override void Link()
            {
                DataContext.PgmLinks.DeleteByPgmId(Pgm.Data.PgmId);
                PgmLink pgmLink = DataContext.PgmLinks.Create();
                pgmLink.PgmId = Pgm.Data.PgmId;
                pgmLink.IncidentId = SelectedIncidentId;
                DataContext.PgmLinks.Save(pgmLink);
                Messenger.Default.Send(new RefreshListMessage<Pgm>(SelectedIncidentId));
                Active = false;
            }

            public override void Unlink()
            {
                DataContext.PgmLinks.DeleteByPgmId(Pgm.Data.PgmId);
                Messenger.Default.Send(new RefreshListMessage<Pgm>(SelectedIncidentId));
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

        public PgmListViewModel(Incident incident)
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
            Messenger.Default.Register<RefreshListMessage<Pgm>>(this, OnRefreshPgmListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitle("Analyses", Incident);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<Link<Pgm>> pgms;
            if (Incident == null)
            {
                pgms = DataContext.GetLinkedPgms().Where(pgm => pgm.Incident == null || !pgm.Incident.Deleted);
            }
            else
            {
                pgms = DataContext.GetLinkedPgms(IncidentId).Select(pgm => new Link<Pgm>(pgm, Incident));
            }
            return CollectionViewSource.GetDefaultView(pgms.OrderBy(pgm => pgm.Data.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Link<Pgm> item)
        {
            yield return item.Data.Name;
            yield return item.Data.Comment;
            yield return item.Data.Author;
            yield return item.IncidentName;
        }

        public void Open()
        {
            Analysis.OpenPgm(DataContext.Project, DataContext.Project.GetPgmById(SelectedItem.Data.PgmId), false, SelectedItem.IncidentId).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected analysis?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.PgmLinks.DeleteByPgmId(SelectedItem.Data.PgmId);
                DataContext.Project.DeletePgm(SelectedItem.Data);
                Messenger.Default.Send(new RefreshListMessage<Pgm>(SelectedItem.IncidentId));
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

        private void OnRefreshPgmListMessage(RefreshListMessage<Pgm> msg)
        {
            if (Incident == null || StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
