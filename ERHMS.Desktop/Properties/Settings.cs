using System;
using System.Threading;

namespace ERHMS.Desktop.Properties
{
    partial class Settings
    {
        private readonly object saveTimerLock = new object();
        private Timer saveTimer;

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

        public override void Save()
        {
            lock (saveTimerLock)
            {
                base.Save();
                saveTimer?.Dispose();
                saveTimer = null;
            }
        }

        public void DeferSave(TimeSpan delay)
        {
            lock (saveTimerLock)
            {
                if (saveTimer == null)
                {
                    saveTimer = new Timer(_ => Save());
                }
                saveTimer.Change(delay, Timeout.InfiniteTimeSpan);
            }
        }

        public bool FlushSave()
        {
            lock (saveTimerLock)
            {
                if (saveTimer == null)
                {
                    return false;
                }
                Save();
                return true;
            }
        }
    }
}
