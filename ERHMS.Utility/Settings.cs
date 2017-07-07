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
        public bool EmailEnableSsl { get; set; }
        public string EmailFrom { get; set; }
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
            EmailEnableSsl = false;
            EmailFrom = null;
            EmailPassword = null;
            MapApplicationId = "Aua5s8kFcEZMx5lsd8Vkerz3frboU1CwzvOyzX_vgSnzsnbqV7xlQ4WTRUlN19_Q";
            OrganizationName = null;
            OrganizationKey = null;
            DataSourcePaths = new HashSet<string>();
        }

        public bool IsEmailConfigured()
        {
            return !string.IsNullOrWhiteSpace(EmailHost) && EmailPort.HasValue && MailExtensions.IsValidAddress(EmailFrom);
        }

        public SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient(EmailHost, EmailPort.Value)
            {
                EnableSsl = EmailEnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            if (string.IsNullOrEmpty(EmailPassword))
            {
                smtpClient.UseDefaultCredentials = true;
            }
            else
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(EmailFrom, EmailPassword);
            }
            return smtpClient;
        }

        public MailMessage GetMailMessage()
        {
            return new MailMessage
            {
                From = new MailAddress(EmailFrom)
            };
        }
    }
}
