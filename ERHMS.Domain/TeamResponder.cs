using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class TeamResponder : Entity
    {
        public string TeamResponderId
        {
            get { return GetProperty<string>(nameof(TeamResponderId)); }
            set { SetProperty(nameof(TeamResponderId), value); }
        }

        public string TeamId
        {
            get { return GetProperty<string>(nameof(TeamId)); }
            set { SetProperty(nameof(TeamId), value); }
        }

        private Team team;
        public Team Team
        {
            get { return team; }
            set { SetProperty(nameof(Team), ref team, value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }

        public string IncidentRoleId
        {
            get { return GetProperty<string>(nameof(IncidentRoleId)); }
            set { SetProperty(nameof(IncidentRoleId), value); }
        }

        private IncidentRole incidentRole;
        public IncidentRole IncidentRole
        {
            get { return incidentRole; }
            set { SetProperty(nameof(IncidentRole), ref incidentRole, value); }
        }

        public TeamResponder()
        {
            TeamResponderId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            TeamResponder clone = (TeamResponder)base.Clone();
            clone.Team = (Team)Team.Clone();
            clone.Responder = (Responder)Responder.Clone();
            clone.IncidentRole = (IncidentRole)IncidentRole.Clone();
            return clone;
        }
    }
}
