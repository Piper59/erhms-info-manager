using ERHMS.Domain;
using System;

namespace ERHMS.Desktop.Properties
{
    partial class Settings
    {
        public bool HasWorkerProjectPath => !string.IsNullOrEmpty(WorkerProjectPath);
        public bool HasIncidentProjectPaths => IncidentProjectPaths.Count > 0;

        public string IncidentProjectPath
        {
            get
            {
                return IncidentProjectPaths[0];
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

        public bool HasProjectPath(CoreProject coreProject)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    return HasWorkerProjectPath;
                case CoreProject.Incident:
                    return HasIncidentProjectPaths;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }

        public string GetProjectPath(CoreProject coreProject)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    return HasWorkerProjectPath ? WorkerProjectPath : null;
                case CoreProject.Incident:
                    return HasIncidentProjectPaths ? IncidentProjectPath : null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }
    }
}
