using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Desktop.Properties
{
    partial class Settings
    {
        public string IncidentProjectPath
        {
            get
            {
                return IncidentProjectPaths.Count == 0 ? null : IncidentProjectPaths[0];
            }
            set
            {
                IncidentProjectPaths.Remove(value);
                IncidentProjectPaths.Insert(0, value);
            }
        }

        public string GetProjectPath(CoreProject coreProject)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    return WorkerProjectPath;
                case CoreProject.Incident:
                    return IncidentProjectPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }

        public Project GetProject(CoreProject coreProject)
        {
            return ProjectExtensions.Open(GetProjectPath(coreProject));
        }

        public View GetView(CoreView coreView)
        {
            return GetProject(coreView.CoreProject).Views[coreView.Name];
        }
    }
}
