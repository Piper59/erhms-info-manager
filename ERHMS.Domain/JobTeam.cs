using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class JobTeam : Entity
    {
        public string JobTeamId
        {
            get { return GetProperty<string>(nameof(JobTeamId)); }
            set { SetProperty(nameof(JobTeamId), value); }
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

        public JobTeam()
        {
            JobTeamId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            JobTeam clone = (JobTeam)base.Clone();
            clone.Job = (Job)Job.Clone();
            clone.Team = (Team)Team.Clone();
            return clone;
        }
    }
}
