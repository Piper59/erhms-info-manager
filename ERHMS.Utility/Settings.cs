using System;
using System.Collections.Generic;

namespace ERHMS.Utility
{
    public class Settings : SettingsBase<Settings>
    {
        public bool InitialExecution { get; set; }
        public string LogLevel { get; set; }
        public HashSet<string> DataSources { get; set; }
        public string RootDirectory { get; set; }
        public string ServiceAddress { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public string EmailSender { get; set; }
        public string MapLicenseKey { get; set; }
        public string OrganizationName { get; set; }
        public Guid? WebSurveyKey { get; set; }

        public Settings()
        {
            InitialExecution = true;
            LogLevel = "DEBUG";
            DataSources = new HashSet<string>();
            RootDirectory = null;
            ServiceAddress = "net.pipe://localhost/erhms-info-manager";
            EmailHost = null;
            EmailPort = 25;
            EmailSender = null;
            MapLicenseKey = "Am2Kmtkt9FKkcW1k9o0NS6NnySTT9JtrAZWeZxwpPP0Ki21n2kyLpIohVd224-uy";
            OrganizationName = null;
            WebSurveyKey = null;
        }
    }
}
