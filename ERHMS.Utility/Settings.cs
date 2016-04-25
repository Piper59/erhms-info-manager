namespace ERHMS.Utility
{
    public class Settings : SettingsBase<Settings>
    {
        public string LogLevel { get; set; }
        public string RootDirectory { get; set; }
        public string ServiceAddress { get; set; }
        public string EmailHost { get; set; }
        public int EmailPort { get; set; }
        public string EmailFromAddress { get; set; }
        public string MapLicenseKey { get; set; }

        public Settings()
        {
#if DEBUG
            LogLevel = "DEBUG";
#else
            LogLevel = "WARN";
#endif
            RootDirectory = "";
            ServiceAddress = "net.pipe://localhost/erhms-info-manager";
            EmailHost = "";
            EmailPort = 25;
            EmailFromAddress = "";
            MapLicenseKey = "Am2Kmtkt9FKkcW1k9o0NS6NnySTT9JtrAZWeZxwpPP0Ki21n2kyLpIohVd224-uy";
        }
    }
}
