using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class Email
    {
        // https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx
        private static readonly Regex Address = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase);

        public static bool IsValidAddress(string address)
        {
            return Address.IsMatch(address);
        }

        public static bool IsConfigured()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.EmailHost))
            {
                return false;
            }
            else if (!Settings.Default.EmailPort.HasValue)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Settings.Default.EmailSender))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static SmtpClient GetClient()
        {
            return new SmtpClient(Settings.Default.EmailHost, Settings.Default.EmailPort.Value);
        }

        public static MailMessage GetMessage()
        {
            return new MailMessage
            {
                From = new MailAddress(Settings.Default.EmailSender)
            };
        }
    }
}
