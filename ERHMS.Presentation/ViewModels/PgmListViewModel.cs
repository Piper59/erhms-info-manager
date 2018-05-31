using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmListViewModel : DocumentViewModel
    {
        public class PgmListChildViewModel : ListViewModel<Pgm>
        {
            public Incident Incident { get; private set; }

            public PgmListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Pgm> GetItems()
            {
                IEnumerable<Pgm> pgms = Incident == null
                    ? Context.Pgms.SelectUndeleted()
                    : Context.Pgms.SelectByIncidentId(Incident.IncidentId);
                return pgms.OrderBy(pgm => pgm.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(pgm => pgm.Incident?.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Pgm item)
            {
                yield return item.Name;
                yield return item.Incident?.Name;
            }
        }

        public Incident Incident { get; private set; }
        public PgmListChildViewModel Pgms { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }

        public PgmListViewModel(Incident incident)
        {
            Title = "Analyses";
            Incident = incident;
            Pgms = new PgmListChildViewModel(incident);
            OpenCommand = new AsyncCommand(OpenAsync, Pgms.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Pgms.HasSelectedItem);
            LinkCommand = new AsyncCommand(LinkAsync, Pgms.HasSelectedItem);
        }

        public async Task OpenAsync()
        {
            Pgm pgm = Context.Pgms.Refresh(Pgms.SelectedItem);
            Wrapper wrapper = Analysis.OpenPgm.Create(pgm.Content, false);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.PgmConfirmDelete, "Delete"))
            {
                Context.PgmLinks.DeleteByPgmId(Pgms.SelectedItem.PgmId);
                Context.Project.DeletePgm(Pgms.SelectedItem.PgmId);
                ServiceLocator.Data.Refresh(typeof(Pgm));
            }
        }

        public async Task LinkAsync()
        {
            PgmLinkViewModel model = new PgmLinkViewModel(Context.Pgms.Refresh(Pgms.SelectedItem));
            await ServiceLocator.Dialog.ShowAsync(model);
        }
    }
}
