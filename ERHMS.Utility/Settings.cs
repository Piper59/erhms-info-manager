using System;
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
        public string ConfigurationFilePath { get; set; }
        public string LogLevelName { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public bool EmailUseSsl { get; set; }
        public string EmailSender { get; set; }
        public string EmailPassword { get; set; }
        public string MapApplicationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationKey { get; set; }
        public HashSet<string> DataSourcePaths { get; set; }

        public void Reset()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LicenseAccepted = false;
            ConfigurationFilePath = null;
            LogLevelName = "DEBUG";
            EmailHost = null;
            EmailPort = 25;
            EmailUseSsl = false;
            EmailSender = null;
            EmailPassword = null;
            MapApplicationId = "Aua5s8kFcEZMx5lsd8Vkerz3frboU1CwzvOyzX_vgSnzsnbqV7xlQ4WTRUlN19_Q";
            OrganizationName = null;
            OrganizationKey = null;
            DataSourcePaths = new HashSet<string>();
        }

        public bool IsEmailConfigured()
        {
            return !string.IsNullOrWhiteSpace(EmailHost) && EmailPort.HasValue && MailExtensions.IsValidAddress(EmailSender);
        }

        public SmtpClient GetSmtpClient(Func<string, string> decrypt)
        {
            SmtpClient client = new SmtpClient(EmailHost, EmailPort.Value)
            {
                EnableSsl = EmailUseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            if (string.IsNullOrEmpty(EmailPassword))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(EmailSender, decrypt(EmailPassword));
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
