using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class JobResponder : Entity
    {
        public string JobResponderId
        {
            get { return GetProperty<string>(nameof(JobResponderId)); }
            set { SetProperty(nameof(JobResponderId), value); }
        }

        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
        }

        private Job job;
        public Job Job
        {
            get { return job; }
            set { SetProperty(nameof(Job), ref job, value); }
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

        public JobResponder()
        {
            JobResponderId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            JobResponder clone = (JobResponder)base.Clone();
            clone.Job = (Job)Job.Clone();
            clone.Responder = (Responder)Responder.Clone();
            clone.IncidentRole = (IncidentRole)IncidentRole.Clone();
            return clone;
        }
    }
}
