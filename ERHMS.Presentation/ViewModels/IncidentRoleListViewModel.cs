using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentRoleListViewModel : ListViewModel<IncidentRole>
    {
        public Incident Incident { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }

        public IncidentRoleListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Roles";
            Incident = incident;
            Refresh();
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
        }

        protected override IEnumerable<IncidentRole> GetItems()
        {
            return Context.IncidentRoles.SelectByIncidentId(Incident.IncidentId).OrderBy(role => role.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(IncidentRole item)
        {
            yield return item.Name;
        }

        public void Add()
        {
            RoleViewModel role = new RoleViewModel(Services, "Add");
            role.Saved += (sender, e) =>
            {
                Context.IncidentRoles.Save(new IncidentRole
                {
                    IncidentId = Incident.IncidentId,
                    Name = role.Name
                });
                MessengerInstance.Send(new RefreshMessage(typeof(IncidentRole)));
            };
            Dialogs.ShowAsync(role);
        }

        public void Edit()
        {
            RoleViewModel role = new RoleViewModel(Services, "Edit")
            {
                Name = SelectedItem.Name
            };
            role.Saved += (sender, e) =>
            {
                SelectedItem.Name = role.Name;
                Context.IncidentRoles.Save(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(IncidentRole)));
            };
            Dialogs.ShowAsync(role);
        }

        public void Remove()
        {
            IncidentRole role = Context.IncidentRoles.SelectById(SelectedItem.IncidentRoleId);
            if (role.IsInUse)
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "The selected role is in use and may not be removed."
                });
            }
            else
            {
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Remove",
                    Message = "Remove the selected role?"
                };
                msg.Confirmed += (sender, e) =>
                {
                    Context.IncidentRoles.Delete(role);
                    MessengerInstance.Send(new RefreshMessage(typeof(IncidentRole)));
                };
                MessengerInstance.Send(msg);
            }
        }
    }
}
