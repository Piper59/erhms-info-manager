using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmListViewModel : ListViewModel<Pgm>
    {
        public Incident Incident { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }

        public PgmListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Analyses";
            Incident = incident;
            Refresh();
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            LinkCommand = new RelayCommand(Link, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                LinkCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Pgm> GetItems()
        {
            IEnumerable<Pgm> pgms;
            if (Incident == null)
            {
                pgms = Context.Pgms.SelectUndeleted();
            }
            else
            {
                pgms = Context.Pgms.SelectByIncidentId(Incident.IncidentId);
            }
            return pgms.OrderBy(pgm => pgm.Name).ThenBy(pgm => pgm.Incident?.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Pgm item)
        {
            yield return item.Name;
            yield return item.Incident?.Name;
        }

        public void Open()
        {
            Pgm pgm = Context.Pgms.SelectById(SelectedItem.PgmId);
            Analysis.OpenPgm.Create(pgm.Content, false).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected analysis?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Context.PgmLinks.DeleteByPgmId(SelectedItem.PgmId);
                Context.Project.DeletePgm(SelectedItem.PgmId);
                MessengerInstance.Send(new RefreshMessage(typeof(Pgm)));
            };
            MessengerInstance.Send(msg);
        }

        public void Link()
        {
            Dialogs.ShowAsync(new PgmLinkViewModel(Services, SelectedItem));
        }
    }
}
