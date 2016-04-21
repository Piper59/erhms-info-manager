using System.Collections.Generic;
using System.Net.Mail;

namespace ERHMS.Utility
{
    public static class Email
    {
        public static void Send(string smtpServer, int smptPort, IEnumerable<string> recipients, string from, string subject, string body, IEnumerable<string> attachments = null)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body
            };
            foreach (string recipient in recipients)
            {
                message.Bcc.Add(recipient);
            }
            if (attachments != null)
            {
                foreach (string attachment in attachments)
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }
            SmtpClient client = new SmtpClient(smtpServer, smptPort);
            client.SendCompleted += (sender, e) => ((SmtpClient)sender).Dispose();
            client.SendAsync(message, null);
        }
    }
}
