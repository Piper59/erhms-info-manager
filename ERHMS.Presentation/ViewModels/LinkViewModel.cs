using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class LinkViewModel<TEntity, TLink> : DialogViewModel
        where TEntity : EpiInfoEntity<TLink>
        where TLink : IncidentEntity
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
                return Context.Incidents.SelectUndeleted().OrderBy(incident => incident.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

        public TEntity Entity { get; private set; }
        public IncidentListChildViewModel Incidents { get; private set; }

        public ICommand LinkCommand { get; private set; }
        public ICommand UnlinkCommand { get; private set; }

        protected LinkViewModel(IServiceManager services, TEntity entity)
            : base(services)
        {
            Title = "Link to Incident";
            Entity = entity;
            Incidents = new IncidentListChildViewModel(services);
            if (entity.Incident != null)
            {
                Incidents.Select(entity.Incident);
            }
            LinkCommand = new Command(Link, Incidents.HasSelectedItem);
            UnlinkCommand = new Command(Unlink);
        }

        public abstract void Link();
        public abstract void Unlink();

        public override void Dispose()
        {
            Incidents.Dispose();
            base.Dispose();
        }
    }
}
