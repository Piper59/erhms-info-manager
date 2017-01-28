using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;

namespace ERHMS.Utility
{
    public class Settings : SettingsBase<Settings>
    {
        public string Version { get; set; }
        public bool LicenseAccepted { get; set; }
        public string LogLevel { get; set; }
        public string RootDirectory { get; set; }
        public HashSet<string> DataSources { get; set; }
        public string ServiceAddress { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public string EmailSender { get; set; }
        public string MapLicenseKey { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationKey { get; set; }

        public Settings()
        {
            Reset();
        }

        public void Reset()
        {
            Version = Assembly.GetExecutingAssembly().GetVersion().ToString();
            LicenseAccepted = false;
            LogLevel = "DEBUG";
            RootDirectory = null;
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
            if (string.IsNullOrWhiteSpace(EmailHost))
            {
                return false;
            }
            else if (!EmailPort.HasValue)
            {
                return false;
            }
            else if (EmailSender == null)
            {
                return false;
            }
            else
            {
                return true;
            }
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
