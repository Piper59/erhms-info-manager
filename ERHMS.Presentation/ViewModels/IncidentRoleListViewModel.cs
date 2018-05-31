using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
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

            public IncidentRoleListChildViewModel(Incident incident)
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

        public IncidentRoleListViewModel(Incident incident)
        {
            Title = "Roles";
            Incident = incident;
            IncidentRoles = new IncidentRoleListChildViewModel(incident);
            AddCommand = new AsyncCommand(AddAsync);
            EditCommand = new AsyncCommand(EditAsync, IncidentRoles.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, IncidentRoles.HasSelectedItem);
            ShowSettingsCommand = new Command(ShowSettings);
        }

        public async Task AddAsync()
        {
            RoleViewModel model = new RoleViewModel("Add");
            model.Saved += (sender, e) =>
            {
                Context.IncidentRoles.Save(new IncidentRole(true)
                {
                    IncidentId = Incident.IncidentId,
                    Name = model.Name
                });
                ServiceLocator.Data.Refresh(typeof(IncidentRole));
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task EditAsync()
        {
            IncidentRole incidentRole = Context.IncidentRoles.Refresh(IncidentRoles.SelectedItem);
            RoleViewModel model = new RoleViewModel("Edit");
            model.Name = incidentRole.Name;
            model.Saved += (sender, e) =>
            {
                incidentRole.Name = model.Name;
                Context.IncidentRoles.Save(incidentRole);
                ServiceLocator.Data.Refresh(typeof(IncidentRole));
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task DeleteAsync()
        {
            IncidentRole incidentRole = Context.IncidentRoles.Refresh(IncidentRoles.SelectedItem);
            if (incidentRole.InUse)
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.IncidentRoleDeleteProhibited);
            }
            else
            {
                if (await ServiceLocator.Dialog.ConfirmAsync(Resources.IncidentRoleConfirmDelete, "Delete"))
                {
                    Context.IncidentRoles.Delete(incidentRole);
                    ServiceLocator.Data.Refresh(typeof(IncidentRole));
                }
            }
        }

        public void ShowSettings()
        {
            ServiceLocator.Document.ShowSettings();
        }
    }
}
