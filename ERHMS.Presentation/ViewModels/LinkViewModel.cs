using ERHMS.Domain;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

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

        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand UnlinkCommand { get; private set; }

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
            LinkCommand = new RelayCommand(Link, Incidents.HasSelectedItem);
            UnlinkCommand = new RelayCommand(Unlink);
            Incidents.SelectionChanged += (sender, e) =>
            {
                LinkCommand.RaiseCanExecuteChanged();
            };
        }

        public abstract void Link();
        public abstract void Unlink();
    }
}
