using ERHMS.EpiInfo.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Domain
{
    public class JobTicket : Entity
    {
        protected override object Id
        {
            get { return null; }
        }

        private Incident incident;
        public Incident Incident
        {
            get { return incident; }
            set { SetProperty(nameof(Incident), ref incident, value); }
        }

        private Job job;
        public Job Job
        {
            get { return job; }
            set { SetProperty(nameof(Job), ref job, value); }
        }

        private Team team;
        public Team Team
        {
            get { return team; }
            set { SetProperty(nameof(Team), ref team, value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }

        private IncidentRole incidentRole;
        public IncidentRole IncidentRole
        {
            get { return incidentRole; }
            set { SetProperty(nameof(IncidentRole), ref incidentRole, value); }
        }

        private ICollection<Location> locations;
        public ICollection<Location> Locations
        {
            get { return locations; }
            set { SetProperty(nameof(Locations), ref locations, value); }
        }

        public string LocationNames
        {
            get { return string.Join(", ", Locations.Select(location => location.Name)); }
        }

        public JobTicket()
            : base(false) { }
    }
}
