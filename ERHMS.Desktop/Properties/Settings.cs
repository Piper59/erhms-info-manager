using ERHMS.Domain;
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

        public void Initialize()
        {
            if (UpgradeRequired)
            {
                Upgrade();
                UpgradeRequired = false;
                Save();
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
    }
}
