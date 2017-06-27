using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace ERHMS.Utility
{
    public partial class Settings
    {
        public string Version { get; set; }
        public bool LicenseAccepted { get; set; }
        public string ConfigurationFile { get; set; }
        public string LogLevel { get; set; }
        public HashSet<string> DataSources { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public bool EmailSsl { get; set; }
        public string EmailSender { get; set; }
        public string EmailPassword { get; set; }
        public string MapLicenseKey { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationKey { get; set; }

        public Settings()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LicenseAccepted = false;
            ConfigurationFile = null;
            Reset();
        }

        public void Reset()
        {
            LogLevel = "DEBUG";
            DataSources = new HashSet<string>();
            EmailHost = null;
            EmailPort = 25;
            EmailSsl = false;
            EmailSender = null;
            EmailPassword = null;
            MapLicenseKey = "Aua5s8kFcEZMx5lsd8Vkerz3frboU1CwzvOyzX_vgSnzsnbqV7xlQ4WTRUlN19_Q";
            OrganizationName = null;
            OrganizationKey = null;
        }

        public bool IsEmailConfigured()
        {
            return !string.IsNullOrWhiteSpace(EmailHost) && EmailPort.HasValue && MailExtensions.IsValidAddress(EmailSender);
        }

        public SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient(EmailHost, EmailPort.Value)
            {
                EnableSsl = EmailSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            if (string.IsNullOrEmpty(EmailPassword))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(EmailSender, EmailPassword);
            }
            return client;
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
