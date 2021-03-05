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
    }
}
