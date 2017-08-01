using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmListViewModel : ListViewModel<Pgm>
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

        public PgmListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Analyses";
            Incident = incident;
            SelectionChanged += (sender, e) =>
            {
                openCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                linkCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
