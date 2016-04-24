namespace ERHMS.Utility
{
    public class Settings : SettingsBase<Settings>
    {
        public string LogLevel { get; set; }
        public string RootDirectory { get; set; }
        public string ServiceAddress { get; set; }

        public Settings()
        {
            LogLevel = "WARN";
            RootDirectory = "";
            ServiceAddress = "net.pipe://localhost/erhms-info-manager";
        }
    }
}
