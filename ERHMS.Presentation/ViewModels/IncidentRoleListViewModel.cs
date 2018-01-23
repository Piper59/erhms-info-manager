using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentRoleListViewModel : DocumentViewModel
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Incident Incident { get; private set; }

            public IncidentRoleListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<IncidentRole> GetItems()
            {
                return Context.IncidentRoles.SelectByIncidentId(Incident.IncidentId)
                    .OrderBy(incidentRole => incidentRole.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(IncidentRole item)
            {
                yield return item.Name;
            }
        }

        public Incident Incident { get; private set; }
        public IncidentRoleListChildViewModel IncidentRoles { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ShowSettingsCommand { get; private set; }

        public IncidentRoleListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Roles";
            Incident = incident;
            IncidentRoles = new IncidentRoleListChildViewModel(services, incident);
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, IncidentRoles.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, IncidentRoles.HasSelectedItem);
            ShowSettingsCommand = new Command(ShowSettings);
        }

        public async Task AddAsync()
        {
            using (RoleViewModel model = new RoleViewModel(Services, "Add"))
            {
                model.Saved += (sender, e) =>
                {
                    Context.IncidentRoles.Save(new IncidentRole(true)
                    {
                        IncidentId = Incident.IncidentId,
                        Name = model.Name
                    });
                    Services.Data.Refresh(typeof(IncidentRole));
                };
                await Services.Dialog.ShowAsync(model);
            }
        }

        public async Task EditAsync()
        {
            IncidentRole incidentRole = Context.IncidentRoles.Refresh(IncidentRoles.SelectedItem);
            using (RoleViewModel model = new RoleViewModel(Services, "Edit"))
            {
                model.Name = incidentRole.Name;
                model.Saved += (sender, e) =>
                {
                    incidentRole.Name = model.Name;
                    Context.IncidentRoles.Save(incidentRole);
                    Services.Data.Refresh(typeof(IncidentRole));
                };
                await Services.Dialog.ShowAsync(model);
            }
        }

        public async Task DeleteAsync()
        {
            IncidentRole incidentRole = Context.IncidentRoles.Refresh(IncidentRoles.SelectedItem);
            if (incidentRole.InUse)
            {
                await Services.Dialog.AlertAsync("The selected role is in use and may not be deleted.");
            }
            else
            {
                if (await Services.Dialog.ConfirmAsync("Delete the selected role?", "Delete"))
                {
                    Context.IncidentRoles.Delete(incidentRole);
                    Services.Data.Refresh(typeof(IncidentRole));
                }
            }
        }

        public void ShowSettings()
        {
            Services.Document.ShowByType(() => new SettingsViewModel(Services));
        }

        public override void Dispose()
        {
            IncidentRoles.Dispose();
            base.Dispose();
        }
    }
}
