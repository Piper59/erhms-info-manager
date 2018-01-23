using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
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

            public PgmListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
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
                return pgms.OrderBy(pgm => pgm.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(pgm => pgm.Incident?.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Pgm item)
            {
                yield return item.Name;
                yield return item.Incident?.Name;
            }
        }

        public PgmListChildViewModel Pgms { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }

        public PgmListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Analyses";
            Pgms = new PgmListChildViewModel(services, incident);
            OpenCommand = new AsyncCommand(OpenAsync, Pgms.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Pgms.HasSelectedItem);
            LinkCommand = new AsyncCommand(LinkAsync, Pgms.HasSelectedItem);
        }

        public async Task OpenAsync()
        {
            Pgm pgm = Context.Pgms.Refresh(Pgms.SelectedItem);
            await Services.Wrapper.InvokeAsync(Analysis.OpenPgm.Create(pgm.Content, false));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected analysis?", "Delete"))
            {
                Context.PgmLinks.DeleteByPgmId(Pgms.SelectedItem.PgmId);
                Context.Project.DeletePgm(Pgms.SelectedItem.PgmId);
                Services.Data.Refresh(typeof(Pgm));
            }
        }

        public async Task LinkAsync()
        {
            using (PgmLinkViewModel model = new PgmLinkViewModel(Services, Context.Pgms.Refresh(Pgms.SelectedItem)))
            {
                await Services.Dialog.ShowAsync(model);
            }
        }

        public override void Dispose()
        {
            Pgms.Dispose();
            base.Dispose();
        }
    }
}
