using ERHMS.Domain;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class LinkViewModel<TEntity, TLink> : DialogViewModel
        where TEntity : LinkedEntity<TLink>
        where TLink : Link
    {
        public class IncidentListChildViewModel : ListViewModel<Incident>
        {
            public IncidentListChildViewModel(IServiceManager services)
                : base(services)
            {
                Refresh();
            }

            protected override IEnumerable<Incident> GetItems()
            {
                return Context.Incidents.SelectUndeleted().OrderBy(incident => incident.Name);
            }

            public void Select(string incidentId)
            {
                SelectedItem = TypedItems.SingleOrDefault(incident => incident.IncidentId.EqualsIgnoreCase(incidentId));
            }
        }

        public TEntity Entity { get; private set; }
        public IncidentListChildViewModel Incidents { get; private set; }

        private RelayCommand linkCommand;
        public ICommand LinkCommand
        {
            get { return linkCommand ?? (linkCommand = new RelayCommand(Link, Incidents.HasSelectedItem)); }
        }

        private RelayCommand unlinkCommand;
        public ICommand UnlinkCommand
        {
            get { return unlinkCommand ?? (unlinkCommand = new RelayCommand(Unlink)); }
        }

        protected LinkViewModel(IServiceManager services, TEntity entity)
            : base(services)
        {
            Title = "Link to Incident";
            Entity = entity;
            Incidents = new IncidentListChildViewModel(services);
            if (entity.Incident != null)
            {
                Incidents.Select(entity.Incident.IncidentId);
            }
            Incidents.SelectionChanged += (sender, e) =>
            {
                linkCommand.RaiseCanExecuteChanged();
            };
        }

        public abstract void Link();
        public abstract void Unlink();
    }
}
