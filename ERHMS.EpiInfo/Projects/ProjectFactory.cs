using System;

namespace ERHMS.EpiInfo.Projects
{
    public static class ProjectFactory
    {
        public static Project GetProject(ProjectType projectType, string path)
        {
            switch (projectType)
            {
                case ProjectType.Worker:
                    return new WorkerProject(path);
                case ProjectType.Incident:
                    return new IncidentProject(path);
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectType));
            }
        }
    }
}
