using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;

namespace ERHMS.Utility
{
    public class Settings : SettingsBase<Settings>
    {
        public string Version { get; set; }
        public string LogLevel { get; set; }
        public bool LicenseAccepted { get; set; }
        public string RootDirectory { get; set; }
        public string ConfigurationFile { get; set; }
        public HashSet<string> DataSources { get; set; }
        // TODO: Remove when Epi Info communication project removed
        public string ServiceAddress { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public string EmailSender { get; set; }
        public string MapLicenseKey { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationKey { get; set; }

        public Settings()
        {
            Version = Assembly.GetExecutingAssembly().GetVersion().ToString();
            LicenseAccepted = false;
            Reset();
        }

        public void Reset()
        {
            LogLevel = "DEBUG";
            RootDirectory = null;
            ConfigurationFile = null;
            DataSources = new HashSet<string>();
            ServiceAddress = "net.pipe://localhost/erhms-info-manager";
            EmailHost = null;
            EmailPort = 25;
            EmailSender = null;
            MapLicenseKey = "Am2Kmtkt9FKkcW1k9o0NS6NnySTT9JtrAZWeZxwpPP0Ki21n2kyLpIohVd224-uy";
            OrganizationName = null;
            OrganizationKey = null;
        }

        public bool IsEmailConfigured()
        {
            return !string.IsNullOrWhiteSpace(EmailHost) && EmailPort.HasValue && MailExtensions.IsValidAddress(EmailSender);
        }

        public SmtpClient GetSmtpClient()
        {
            return new SmtpClient(EmailHost, EmailPort.Value);
        }

        public MailMessage GetMailMessage()
        {
            return new MailMessage
            {
                From = new MailAddress(EmailSender)
            };
        }
    }
}
